using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureListener : MonoBehaviour
{
    private Button button;

    private void OnEnable()
    {
        GestureHandler.OnTap += new GestureHandler.GestureHandlerDelegate(PressButton);
    }

    private void OnDisable()
    {
        GestureHandler.OnTap -= new GestureHandler.GestureHandlerDelegate(PressButton);
    }

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void PressButton()
    {
        button.onClick.Invoke();
    }
}