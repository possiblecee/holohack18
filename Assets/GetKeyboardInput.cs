using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.UI.Keyboard;

public class GetKeyboardInput : MonoBehaviour
{
    KeyboardInputField inputField;

    // Use this for initialization
    void Start()
    {
        inputField = GetComponent<KeyboardInputField>();
    }

    public void SetUrl()
    {
        URLSingleton.Instance.url = inputField.text;
    }
}