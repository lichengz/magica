using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    private bool readyToNextSentence;

    void Start()
    {
        StartCoroutine(Type());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && readyToNextSentence)
        {
            NextSentence();
        }
        if (textDisplay.text == sentences[index])
        {
            readyToNextSentence = true;
        }
    }
    IEnumerator Type()
    {
        readyToNextSentence = false;
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }
    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            readyToNextSentence = false;
        }
    }
}
