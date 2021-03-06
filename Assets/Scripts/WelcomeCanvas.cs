﻿using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeCanvas : MonoBehaviour
{
    [Tooltip("Distance from camera to keep the object while placing it.")]
    public float DefaultGazeDistance = 3.0f;

    private Interpolator interpolator;

    private Vector3 defaultScale;

    // Use this for initialization
    private void Start()
    {
        interpolator = this.GetComponent<Interpolator>();
        if (interpolator == null)
        {
            interpolator = this.gameObject.AddComponent<Interpolator>();
        }

        defaultScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        SetOnSurface();
    }

    private void SetOnSurface()
    {
        var mainCam = Camera.main;
        var headPosition = mainCam.transform.position;
        var gazeDirection = mainCam.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
            DefaultGazeDistance, SpatialMappingManager.Instance.LayerMask))
        {
            // Move this object's parent object to
            // where the raycast hit the Spatial Mapping mesh.
            interpolator.SetTargetPosition(hitInfo.point + (gazeDirection * -0.1f));
        }
        else
        {
            Vector3 pos = headPosition + gazeDirection * DefaultGazeDistance;
            interpolator.SetTargetPosition(pos);
        }

        // billboard - rotation lock around Y
        Quaternion toQuat = Camera.main.transform.localRotation;
        toQuat.x = 0;
        toQuat.z = 0;
        interpolator.SetTargetRotation(toQuat);
    }
}
