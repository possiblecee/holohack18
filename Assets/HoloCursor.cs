using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloCursor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    private Camera mainCamera;

    private Vector3 headPosition;
    private Vector3 gazeDirection;

    private RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        headPosition = mainCamera.transform.position;
        gazeDirection = mainCamera.transform.forward;

        if(Physics.Raycast(headPosition, gazeDirection, out hit))
        {
            meshRenderer.enabled = true;

            transform.position = hit.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}