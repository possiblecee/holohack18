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
    public GestureRecognizer recognizer;
    public EventHandler<EventArgs> onObjectPlaced;
    public void NotifyOnObjectPlace(){
        if(onObjectPlaced != null)
            onObjectPlaced(this, new EventArgs());
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    public void SetPlace(bool enable)
    {
        // On each Select gesture, toggle whether the user is in placing mode.
        placing = enable;
        Debug.Log("PLACING: "+placing);
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
            recognizer.Tapped -=  Recognizer_Tapped;
        }
    }

    private IEnumerator EnablePlaceGesture()
    {
        yield return new WaitForSeconds(1.0f);
        recognizer.Tapped +=  Recognizer_Tapped;
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

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                30.0f, SpatialMappingManager.Instance.LayerMask))
            {
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                //Debug.Log("HITT!!!");
                this.transform.position = hitInfo.point + (gazeDirection * -0.01f);

                if(rotationType == RotationType.NORMAL) {
                    // Rotate this object's parent object to face the user.
                    Quaternion toQuat = mainCam.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    this.transform.forward = new Vector3(hitInfo.normal.x, hitInfo.normal.y, hitInfo.normal.z);
                }
                if(rotationType == RotationType.CAMERA) {
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    this.transform.rotation = toQuat;
                }
            }
        }
    }
}

public enum RotationType{
    CAMERA,
    NORMAL
}