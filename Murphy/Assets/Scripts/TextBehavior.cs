using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextBehavior : MonoBehaviour
{
    public Text displayText;
    public Text displayText2;

    float sinceLast;

    void Start()
    {
        displayText.text = " ";
    }

    //Text maker
    public void SetText(string textIn)
    {
        displayText.text = textIn;
        sinceLast = Time.time;
    }

    //When speech is finished, empty the queue.
    public IEnumerator shiftSentence()
    {
        displayText2.text = displayText.text;

        yield return new WaitForSeconds(2);
        displayText2.text = " ";
        yield return new WaitForSeconds(2);
        if(Time.time - sinceLast >= 2)
        {
            displayText.text = " ";
        }
    }

    public bool isEmpty()
    {
        if (displayText2.text == " " && displayText.text == " ")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
