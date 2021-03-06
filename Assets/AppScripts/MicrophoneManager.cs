﻿using HoloToolkit;
using System.Collections;
using System.Text;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity.InputModule;

public class MicrophoneManager : MonoBehaviour {
    [Tooltip("A text area for the recognizer to display the recognized strings.")]
    public GameObject Canvas;

    private TextBehavior behave;

    private DictationRecognizer dictationRecognizer;

    // Use this string to cache the text currently displayed in the text box.
    private StringBuilder textSoFar;

    // Using an empty string specifies the default microphone. 
    private static string deviceName = string.Empty;
    private int samplingRate;
    private const int messageLength = 10;

    // Use this to reset the UI once the Microphone is done recording after it was started.
    private bool hasRecordingStarted;

    bool isRec = false;

    void Awake() {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        int unused;
        Microphone.GetDeviceCaps(deviceName, out unused, out samplingRate);

        // Use this string to cache the text currently displayed in the text box.
        textSoFar = new StringBuilder();

        // Use this to reset the UI once the Microphone is done recording after it was started.
        hasRecordingStarted = false;
    }

    void Start() {
        //behave = Canvas.GetComponent<TextBehavior>();
    }

    void Update() {

        //If there's any text, display it.
        if ( textSoFar.Length > 0 ) {
            //Canvas.GetComponentInChildren<CanvasRenderer>().SetAlpha(1);
            Debug.Log(textSoFar);
        }
        /*if ( isRec ) {
            this.gameObject.GetComponent<Renderer>().material.color = Color.green;
        } else {
            this.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        */

        // 3.a: Add condition to check if dictationRecognizer.Status is Running
        if ( hasRecordingStarted && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running ) {
            // Reset the flag now that we're cleaning up the UI.
            hasRecordingStarted = false;

            // This acts like pressing the Stop button and sends the message to the Communicator.
            // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
            // Look at the StopRecording function.
            SendMessage("RecordStop");
        }
    }

    /// <summary>
    /// Turns on the dictation recognizer and begins recording audio from the default microphone.
    /// </summary>
    /// <returns>The audio clip recorded from the microphone.</returns>
    public AudioClip StartRecording() {
        // 3.a Shutdown the PhraseRecognitionSystem. This controls the KeywordRecognizers
        PhraseRecognitionSystem.Shutdown();

        //this.GetComponent<ParticleSystem>().Play();

        Debug.Log("RECORDING START");
        // 3.a: Start dictationRecognizer
        dictationRecognizer.Start();

        // 3.a Uncomment this line
        //DictationDisplay.text = "Dictation is starting. It may take time to display your text the first time, but begin speaking now...";

        // Set the flag that we've started recording.
        hasRecordingStarted = true;
        isRec = true;

        // Start recording from the microphone for 10 seconds.
        return Microphone.Start(deviceName, false, messageLength, samplingRate);
    }

    /// <summary>
    /// Ends the recording session.
    /// </summary>
    public void StopRecording() {

        // 3.a: Check if dictationRecognizer.Status is Running and stop it if so
        if ( dictationRecognizer.Status == SpeechSystemStatus.Running ) {
            Debug.Log("I'm here in stop");
            dictationRecognizer.Stop();
            textSoFar.Length = 0;
        }
        textSoFar.Length = 0;
        //this.GetComponent<ParticleSystem>().Play();
        Microphone.End(deviceName);
        isRec = false;
    }

    /// <summary>
    /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
    /// </summary>
    /// <param name="text">The currently hypothesized recognition.</param>
    private void DictationRecognizer_DictationHypothesis(string text) {
        // 3.a: Set DictationDisplay text to be textSoFar and new hypothesized text
        // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event
        Debug.Log(textSoFar.ToString() + " " + text + "...");

        behave.SetText(text);
    }

    /// <summary>
    /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
    /// </summary>
    /// <param name="text">The text that was heard by the recognizer.</param>
    /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence) {
        // 3.a: Append textSoFar with latest text
        textSoFar.Append(text + ". ");

        Debug.Log(textSoFar);

        StartCoroutine(behave.shiftSentence());
    }

    /// <summary>
    /// This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
    /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
    /// </summary>
    /// <param name="cause">An enumerated reason for the session completing.</param>
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause) {
        // If Timeout occurs, the user has been silent for too long.
        // With dictation, the default timeout after a recognition is 20 seconds.
        // The default timeout with initial silence is 5 seconds.

        isRec = false;

        if ( cause == DictationCompletionCause.TimeoutExceeded ) {
            Microphone.End(deviceName);

            SendMessage("ResetAfterTimeout");
        }
    }

    /// <summary>
    /// This event is fired when an error occurs.
    /// </summary>
    /// <param name="error">The string representation of the error reason.</param>
    /// <param name="hresult">The int representation of the hresult.</param>
    private void DictationRecognizer_DictationError(string error, int hresult) {
        // 3.a: Set DictationDisplay text to be the error string
    }

    private IEnumerator RestartSpeechSystem(KeywordManager keywordToStart) {
        while ( dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running ) {
            yield return null;
        }

        keywordToStart.StartKeywordRecognizer();
    }
}