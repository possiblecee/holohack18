using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
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

    [Tooltip("Distance from camera to keep the object while placing it.")]
    public float DefaultGazeDistance = 2.0f;

    public float DefaultOffset = 0;

    private Interpolator interpolator;

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

    private void Start()
    {
        interpolator = this.GetComponent<Interpolator>();
        if (interpolator == null)
        {
            interpolator = this.gameObject.AddComponent<Interpolator>();
        }
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
                SetOnSurface();
            }
            else
            {
                SetOnGround();
            }
        }
    }

    private void SetOnSurface()
    {
        var headPosition = mainCam.transform.position;
        var gazeDirection = mainCam.transform.forward;

        RaycastHit hitInfo;
        bool hasHit = false;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
            DefaultGazeDistance, SpatialMappingManager.Instance.LayerMask))
        {
            // Move this object's parent object to
            // where the raycast hit the Spatial Mapping mesh.
            interpolator.SetTargetPosition(hitInfo.point + (gazeDirection * -0.01f));
            hasHit = true;
        }
        else
        {
            Vector3 pos = headPosition + gazeDirection * DefaultGazeDistance;
            interpolator.SetTargetPosition(pos);
        }

        if (rotationType == RotationType.NORMAL && hasHit)
        {
            // Rotate this object's parent object to face the user.
            interpolator.SetTargetRotation(Quaternion.LookRotation(-hitInfo.normal, Vector3.up));
        }
        else
        {
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            interpolator.SetTargetRotation(toQuat);
        }
    }

    private void SetOnGround()
    {
        var headPosition = mainCam.transform.position;
        var gazeDirection = mainCam.transform.forward;

        Vector3 pos = headPosition + gazeDirection * DefaultGazeDistance;
        pos.y = GetGround() + DefaultOffset;
        interpolator.SetTargetPosition(pos);

        Quaternion toQuat = Camera.main.transform.localRotation;
        toQuat.x = 0;
        toQuat.z = 0;
        interpolator.SetTargetRotation(toQuat);
    }

    private float GetGround()
    {
        if(ground != 0)
        {
            return ground;
        }

        RaycastHit hitinfo;
        if (Physics.BoxCast(new Vector3(0, -10, 0), new Vector3(5, 0.1f, 5), Vector3.up, out hitinfo, Quaternion.identity, 30f, SpatialMappingManager.Instance.LayerMask))
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