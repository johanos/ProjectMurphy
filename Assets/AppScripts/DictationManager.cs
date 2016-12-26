using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class DictationManager : MonoBehaviour {

    Queue textQueue = new Queue();
    [Tooltip("A text area for the recognizer to display the recognized strings.")]
    public Text DictationDisplay;

    [Tooltip("GameObject that will receive on and off notifications.")]
    public GameObject Trigger;

    [SerializeField]
    public StringBuilder result = new StringBuilder();

    bool running = false;
    private DictationRecognizer m_DictationRecognizer;

    void Update() {
        TriggerController trigger = Trigger.GetComponent<TriggerController>();
        if ( running ) {
            trigger.UIStateRunning();
        }
        else {
            trigger.UIStateStopped();
        }
    }

    void Start() {
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            result = new StringBuilder();
            result.Append(text);
            textQueue.Clear();
            FormatStringForDisplay(result.ToString());
            DisplaySubtitles();
    
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            result = new StringBuilder();
            result.Append(text);
            textQueue.Clear();
            FormatStringForDisplay(result.ToString());
            DisplaySubtitles();

        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if ( completionCause != DictationCompletionCause.Complete )
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);

            running = false;
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            running = false;
        };
    }

    public void StartRecording() {
        m_DictationRecognizer.Start();
        Debug.Log("Dictation Started");
        running = true;
    }

    public void StopRecording() {
        if (m_DictationRecognizer.Status == SpeechSystemStatus.Running ) {
            m_DictationRecognizer.Stop();
        }
        Debug.Log("Dictation Ended");
        running = false; 
    }

    public void FormatStringForDisplay(string textToFormat) {
        StringBuilder newString = new StringBuilder();
        
        for (int i = 0; i < textToFormat.Length; i++ ) {
            if (i % 36 == 0 && i != 0) {
                newString.Append('\n');
                textQueue.Enqueue(newString.ToString());
                newString = new StringBuilder();
            }
            newString.Append(textToFormat[i]);
        }
        textQueue.Enqueue(newString.ToString());
    }
    public void DisplaySubtitles() {
        DictationDisplay.text = "";
        if (textQueue.Count < 3 ) {
            //just barf out what you have no need to wait. 
            while ( textQueue.Count > 0 ) {
                DictationDisplay.text += (string)textQueue.Dequeue();
            }
        }
        else {

            for (int i = 0; i < 3; i++ ) {
                DictationDisplay.text += (string)textQueue.Dequeue();
            }

            while (textQueue.Count > 0 ) {
                //remove 36 characters and then make the text start there...
                DictationDisplay.text = DictationDisplay.text.Substring(37);
                DictationDisplay.text += (string)textQueue.Dequeue();
            }
        }
    }
}