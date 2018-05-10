using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GestureHandler : MonoBehaviour
{
    public delegate void GestureHandlerDelegate();
    public static event GestureHandlerDelegate OnTap;

    public static GestureHandler Instance { get; set; }
    public GameObject focusedObject { get; private set; }

    private Camera mainCamera;
    private GestureRecognizer gestureRecognizer;

    private void Awake()
    {
        Instance = this;
        gestureRecognizer = new GestureRecognizer();

        gestureRecognizer.Tapped += (args) =>
        {
            if (focusedObject != null)
            {
                if (OnTap != null)
                {
                    OnTap();
                }
            }
        };
        gestureRecognizer.StartCapturingGestures();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private GameObject prevFocusedObject;
    private RaycastHit hit;

    void Update()
    {
        Vector3 headPosition = mainCamera.transform.position;
        Vector3 gazeDirection = mainCamera.transform.forward;

        if (Physics.Raycast(headPosition, gazeDirection, out hit))
        {
            focusedObject = hit.collider.gameObject;
        }
        else
        {
            focusedObject = null;
        }

        if (focusedObject != prevFocusedObject)
        {
            gestureRecognizer.CancelGestures();
            gestureRecognizer.StartCapturingGestures();
        }
    }

}