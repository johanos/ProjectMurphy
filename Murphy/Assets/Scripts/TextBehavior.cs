using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextBehavior : MonoBehaviour
{
    public Text displayText1;
    public Text displayText2;
    public Text displayText3;

    string sentence;

    // public delegate void TextBehaviorDelegate(TextBehavior , string e);

    // Set Queue Capacity  = 5
    public int lineCapacity = 10;
    Queue<string> TextQ;

    void Start()
    {
        // Initialize Queue, probably ""
        TextQ = new Queue<string>(lineCapacity);
        TextQ.Enqueue("Hello!");
    }

    public void SetText(string textIn)
    {
        // Set for Queue Capacity
        if (TextQ.Count >= lineCapacity)
        {
            TextQ.Clear();
            displayText3.text = displayText2.text;
            displayText2.text = displayText1.text;
        }

        TextQ.Enqueue(textIn);
    }

    void Update()
    {
        // Input arg will be real-time speech-to-text string 

        foreach (string word in TextQ)
        {
            sentence += " " + word;
        }
        displayText1.text = sentence;
        sentence = " ";

    }
}
