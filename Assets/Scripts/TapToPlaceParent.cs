using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class TapToPlaceParent : MonoBehaviour
{
    bool placing = false;
    public Camera mainCam;
    public RotationType rotationType;
    public PositionType positionType;
    public GestureRecognizer recognizer;
    public EventHandler<EventArgs> onObjectPlaced;
    public void NotifyOnObjectPlace()
    {
        if (onObjectPlaced != null)
            onObjectPlaced(this, new EventArgs());
    }

    private float ground = 0;

    // Called by GazeGestureManager when the user performs a Select gesture
    public void SetPlace(bool enable)
    {
        // On each Select gesture, toggle whether the user is in placing mode.
        placing = enable;
        Debug.Log("PLACING: " + placing);
        // If the user is in placing mode, display the spatial mapping mesh.
        if (placing)
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = true;
            StartCoroutine(EnablePlaceGesture());
        }
        // If the user is not in placing mode, hide the spatial mapping mesh.
        else
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = false;
            recognizer.Tapped -= Recognizer_Tapped;
        }
    }

    private IEnumerator EnablePlaceGesture()
    {
        yield return new WaitForSeconds(1.0f);
        recognizer.Tapped += Recognizer_Tapped;
    }

    private void Recognizer_Tapped(TappedEventArgs obj)
    {
        SetPlace(false);
        NotifyOnObjectPlace();
    }

    public void OnClick()
    {
        Debug.Log("I'm clicked!");
        SetPlace(true);
    }


    // Update is called once per frame
    void Update()
    {
        // If the user is in placing mode,
        // update the placement to match the user's gaze.

        if (placing)
        {
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = mainCam.transform.position;
            var gazeDirection = mainCam.transform.forward;

            if (positionType == PositionType.NORMAL)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, SpatialMappingManager.Instance.LayerMask))
                {
                    // Move this object's parent object to
                    // where the raycast hit the Spatial Mapping mesh.
                    Debug.Log("HITT!!!");
                    this.transform.position = hitInfo.point + (gazeDirection * -0.01f);

                    if (rotationType == RotationType.NORMAL)
                    {
                        // Rotate this object's parent object to face the user.
                        Quaternion toQuat = mainCam.transform.localRotation;
                        toQuat.x = 0;
                        toQuat.z = 0;
                        this.transform.forward = new Vector3(hitInfo.normal.x, hitInfo.normal.y, hitInfo.normal.z);
                    }
                    if (rotationType == RotationType.CAMERA)
                    {
                        Quaternion toQuat = Camera.main.transform.localRotation;
                        toQuat.x = 0;
                        toQuat.z = 0;
                        this.transform.rotation = toQuat;
                    }
                }
            }
            else
            {
                float distance = 3f;
                Vector3 pos = headPosition + gazeDirection * distance;
                pos.y = GetGround();
                this.transform.position = pos;

                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
                this.transform.rotation = toQuat;
            }
        }
    }

    private float GetGround()
    {
        if(ground != 0)
        {
            return ground;
        }

        RaycastHit hitinfo;
        if (Physics.BoxCast(new Vector3(0, -10, 0), new Vector3(5, 1, 5), Vector3.up, out hitinfo))
        {
            ground = hitinfo.point.y;
            Debug.Log("Ground detected: " + ground);
            return ground;
        }
        else
        {
            return 0;
        }
    }
}

public enum RotationType
{
    CAMERA,
    NORMAL
}

public enum PositionType
{
    NORMAL,
    GROUND
}