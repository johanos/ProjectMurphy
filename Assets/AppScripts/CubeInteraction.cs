using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System;

public class CubeInteraction : MonoBehaviour, IFocusable, IInputClickHandler {


    // Use this for initialization
    void Start () {
        
	}

    // Update is called once per frame
    void Update () {
	
	}

    public void OnInputClicked(InputEventData eventData) {
        Debug.Log("NERD");
        TriggerController controller = gameObject.GetComponent<TriggerController>();
        controller.MicrophoneClicked();
    }

    public void OnFocusEnter() {
        //Debug.Log("Entered Focus");
    }

    public void OnFocusExit() {
        //Debug.Log("Exited Focus");
    }
}
