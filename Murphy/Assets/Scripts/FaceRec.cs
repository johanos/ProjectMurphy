using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
public class FaceRec : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    CameraParameters cameraParameters;
    Vector3 cameraPosition;
    Quaternion cameraRotation;
    Vector3 capturePosition;
    Vector3 captureForward;
    public GameObject textPrefab;
    public GameObject textObject = null;
    // Use this for initialization
    void Start() {
        Debug.Log("Merp I started");
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.JPEG;
        Debug.Log("Resolution = (" + cameraResolution.width.ToString() + ", " + cameraResolution.height + ")");
        StartCoroutine(captureLoop());
    }
    IEnumerator captureLoop() {
        while ( true ) {
            // Create a PhotoCapture object
            PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
                photoCaptureObject = captureObject;
            });
            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, false, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                capturePosition = Camera.main.transform.position;
                captureForward = Camera.main.transform.forward;
            });
            //FaceAPI trial allows only 20 calls per minute
            //60/3.1 = 19.35 calls per minute
            yield return new WaitForSeconds(3.2f);
        }
    }
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        // Copy Raw img to byte list
        List<byte> buff = new List<byte>();
        photoCaptureFrame.CopyRawImageDataIntoBuffer(buff);
        // Get Spatial Data
        var cameraToWorldMatrix = new Matrix4x4();
        photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
        var projectionMatrix = new Matrix4x4();
        photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);
        cameraPosition = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);
        cameraRotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));
        // Make Post Request to Cognitive Services Face API
        StartCoroutine(PostToAPI(buff.ToArray(), projectionMatrix, cameraToWorldMatrix));
        // Deactivate our camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }
    IEnumerator PostToAPI(byte[] imgData, Matrix4x4 projection, Matrix4x4 camToWorld) {
        var url = "https://api.projectoxford.ai/face/v1.0/detect?returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses";
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", "8237559739874b4480d0003e1bd251b8" },
            { "Content-Type", "application/octet-stream" }
        };
        Debug.Log(string.Format("cam {0}", cameraPosition));
        Debug.Log(string.Format("proj {0}", projection));
        Debug.Log(string.Format("cam2world {0}", camToWorld));
        Debug.Log("Image Size: " + imgData.Length + " bytes");
        WWW www = new WWW(url, imgData, headers);
        yield return www;
        string responseString = www.text;
        JSONObject j = new JSONObject(responseString);
        Debug.Log(j);
        //if it's empty, sucks
        if ( j.list.Count == 0 ) {
            Debug.Log("No faces found :<");
            textObject = null;
        } else {
            //Process the faces, spawn text.
            spawnAtFace(j, projection, camToWorld);
        }
    }
    void spawnAtFace(JSONObject faces, Matrix4x4 projection, Matrix4x4 camToWorld) {
        var result = faces.list.First();
        if ( textObject == null ) {
            textObject = (GameObject)Instantiate(textPrefab);
        }
        var rect = result.GetField("faceRectangle");
        var top = rect.GetField("top").i;
        var left = rect.GetField("left").i;
        var width = rect.GetField("width").i;
        var height = rect.GetField("height").i;
        //Calculate offset and transform it back to 3d space

        Vector3 infoOffsetPoint = new Vector3((left) * (Screen.width / cameraParameters.cameraResolutionWidth), (top) * (Screen.width / cameraParameters.cameraResolutionWidth), 20);
        Debug.Log(string.Format("info offset {0}", infoOffsetPoint));
        infoOffsetPoint = projection.MultiplyPoint3x4(infoOffsetPoint);
        Debug.Log(string.Format("info offset {0}", infoOffsetPoint));
        Vector3 offset = camToWorld.MultiplyPoint3x4(infoOffsetPoint);
        Debug.Log(string.Format("offset {0}", offset));
        //Set position of chatbox and tag it
        Vector3 position = cameraPosition + offset;
        Debug.Log("Chatbox position: " + position);
        Debug.Log("User position in space: " + Camera.main.transform.position);
        textObject.transform.position = position;
        textObject.transform.rotation.SetLookRotation(Camera.main.transform.position);
    }
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}