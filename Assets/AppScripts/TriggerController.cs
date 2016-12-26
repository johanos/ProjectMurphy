using UnityEngine;
using System.Collections;

public class TriggerController : MonoBehaviour {
    bool isOn = false;
    public GameObject DictationManager;
    [Tooltip("Material for ON state")]
    public Material OnColor;

    [Tooltip("Material for OFF state")]
    public Material OffColor;

    [Tooltip("Background Game Object")]
    public GameObject Background;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MicrophoneClicked() {
        DictationManager manny = this.DictationManager.GetComponent<DictationManager>();
        Debug.Log("Here. State: " + isOn);
        if ( isOn ) {
            isOn = false;
            manny.StopRecording();
        } else {
            isOn = true;
            manny.StartRecording();
        }
        //this needs to get the DictationManager game object and tell it to begin. 
    }

    public void UIStateRunning() {
        Renderer bgRenderer = Background.GetComponent<Renderer>();
        bgRenderer.material = OnColor;
    }

    public void UIStateStopped() {
        Renderer bgRenderer = Background.GetComponent<Renderer>();
        bgRenderer.material = OffColor;
    }
}
