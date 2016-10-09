using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextBehavior : MonoBehaviour
{
    public Text displayText;

    string sentence;

    public int lineCapacity = 10;
    private int lineCount;
    Queue<string> TextQ;
    private int count = 0;

    void Start()
    {
        // Initialize Queue, probably ""
        TextQ = new Queue<string>(2*lineCapacity);
        displayText.text = "";
        lineCount = 0;
    }

    //Text maker
    public void SetText(string textIn)
    {
        //Before anything, make sure queue has space
        if(TextQ.Count == 2*lineCapacity)
        {
            TextQ.Dequeue();
        }

        // Input arg will be real-time speech-to-text string 
        TextQ.Enqueue(textIn);

        foreach (string word in TextQ)
        {
            lineCount++;
            sentence += " " + word;
            if(lineCount == lineCapacity)
            {
                sentence += '\n';
                lineCount = 0;
            }
        }
        lineCount = 0;
        displayText.text = sentence;
        sentence = " ";
    }

    //Test input
    void Update()
    {
        if (count % 20 == 0)
        {

            SetText("word" + count);
        }

        count++;
    }

    //When speech is finished, empty the queue.
    public IEnumerator emptyQueue()
    {
        while(TextQ.Count > 0)
        {
            TextQ.Dequeue();
            yield return new WaitForSeconds(2.25f);
        }
    }

    public bool isEmpty()
    {
        if(TextQ.Count != 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
