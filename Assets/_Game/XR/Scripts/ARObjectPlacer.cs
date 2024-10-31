using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class ARObjectPlacer : MonoBehaviour
{
    public GameObject objectPrefab;
    private GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += OnTouchDown;
    }

    void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= OnTouchDown;
    }
    void OnTouchDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;
        if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(objectPrefab, hitPose.position, hitPose.rotation);
                spawnedObject.transform.localScale = Vector3.one;
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
}
