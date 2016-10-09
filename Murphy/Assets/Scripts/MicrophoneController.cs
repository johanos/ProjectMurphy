using UnityEngine;
using System.Collections;

public class MicrophoneController : MonoBehaviour
{

    private MicrophoneManager microphoneManager;
    private bool recordingOnGoing; 
	// Use this for initialization
	void Start ()
	{
	    Debug.Log("Hello");
	    recordingOnGoing = false;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnSelect()
    {
        
        microphoneManager = gameObject.GetComponent<MicrophoneManager>();
        Debug.Log("Selected");
        if (!recordingOnGoing)
        {
            microphoneManager.StartRecording();
            recordingOnGoing = true;

        }
        else
        {
            microphoneManager.StopRecording();
            recordingOnGoing = false;
        }

    }
}
