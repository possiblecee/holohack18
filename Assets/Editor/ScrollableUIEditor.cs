using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScrollableUI))]
public class ScrollableUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScrollableUI _scrollableUI = (ScrollableUI)target;

        if (GUILayout.Button("LoadImage"))
        {
            _scrollableUI.GetImage();
        }

        if (GUILayout.Button("Next"))
        {
            _scrollableUI.NextObject();
        }

        if (GUILayout.Button("Prev"))
        {
            _scrollableUI.PrevObject();
        }
    }

}