using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextBehavior : MonoBehaviour {
    public Text displayText;
    public Text displayText2;

    string[] wordsIn;
    string sentence;

    float sinceLast;

    void Start() {
        displayText.text = " ";
        displayText2.text = " ";
    }

    //Text maker
    public void SetText(string textIn) {
        wordsIn = textIn.Split(' ');

        sentence = " ";

        if ( wordsIn.Length > 8 ) {
            for ( int i = wordsIn.Length - 8; i < wordsIn.Length; i++ ) {
                sentence += wordsIn[i] + " ";
            }
        } else {
            foreach ( string word in wordsIn ) {
                sentence += word + " ";
            }
        }
        displayText.text = sentence;
        sinceLast = Time.time;
    }

    //When speech is finished, empty the queue.
    public IEnumerator shiftSentence() {
        displayText2.text = displayText.text;
        displayText.text = " ";

        yield return new WaitForSeconds(2);
        displayText2.text = " ";
        yield return new WaitForSeconds(2);
        if ( Time.time - sinceLast >= 2 ) {
            displayText.text = " ";
        }
    }

    public bool isEmpty() {
        if ( displayText2.text == " " && displayText.text == " " ) {
            return true;
        } else {
            return false;
        }
    }
}
