using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyTextButtonHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField consoleInputField;
    [SerializeField] Button copyLogButton;

    void Start()
    {
        copyLogButton.onClick.AddListener(OnCopyLogButtonClick);
    }

    void OnCopyLogButtonClick()
    {
        // Copy the text from the console input field to the clipboard
        GUIUtility.systemCopyBuffer = consoleInputField.text;
        Debug.Log("Log copied to clipboard.");
    }
}
