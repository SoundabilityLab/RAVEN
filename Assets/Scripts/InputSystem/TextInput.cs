using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GoogleTextToSpeech.Scripts;

public class TextInput : InputSource
{
    [SerializeField] private TMP_InputField inputField;
    private TextToSpeech textToSpeech;
    private InputManager inputManager;
    public override void EndRecord()
    {
        Debug.Log("end Text entry!");
        textToSpeech = FindObjectOfType<TextToSpeech>();
        inputManager = FindObjectOfType<InputManager>();
        if(textToSpeech != null && inputManager != null && !inputManager.useMic){
            textToSpeech.PlayTtsAudio("You entered: " + inputField.text);
        }
        if(inputManager != null){
            inputManager.mostRecentMessage = inputField.text;
        }
        Timer.Instance.request = inputField.text;
        requestObject.SetMessage(inputField.text);
        inputField.DeactivateInputField();
        inputField.gameObject.SetActive(false);
        requestObject.CloseChannel();
    }

    public override void AbortRecord()
    {
        textToSpeech = FindObjectOfType<TextToSpeech>();
        if(textToSpeech != null){
            textToSpeech.PlayTtsAudio("Prompt Entry Box Closed.");
        }
        inputField.DeactivateInputField();
        inputField.gameObject.SetActive(false);
        requestObject.CloseChannel(aborted:true);
    }

    protected override void SetupRecord()
    {
        Debug.Log("text entry!");
        inputField.gameObject.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
    }
}
