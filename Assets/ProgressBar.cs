using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private bool canRotate;

    [SerializeField]
    private float rotationSpeed;

    private RectTransform rectTransform;
    private Vector3 rotationDirection;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rotationDirection = new Vector3(0, 0, -1) * rotationSpeed;
    }

    private void Update()
    {
        RotateBar();
    }

    private void RotateBar()
    {
        if (canRotate) rectTransform.Rotate(rotationDirection);
    }

    public void OnLoadingFinished()
    {
        gameObject.SetActive(false);
    }
}