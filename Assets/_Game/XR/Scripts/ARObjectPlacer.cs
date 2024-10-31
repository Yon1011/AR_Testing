using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class ARObjectPlacer : MonoBehaviour
{
    [SerializeField] protected Camera arCamera;
    [SerializeField] protected float placingDepth = 1f;
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
        planeManager.planesChanged += OnPlaneChanged;
    }

    void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= OnTouchDown;
        planeManager.planesChanged += OnPlaneChanged;
    }
    void OnTouchDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;
        if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (spawnedObject != null)
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }

    void OnPlaneChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count > 0 && spawnedObject == null)
        {
            Vector2 midScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            var placingPos = arCamera.ScreenToWorldPoint(new Vector3(midScreen.x, midScreen.y, placingDepth));
            spawnedObject = Instantiate(objectPrefab);
            spawnedObject.transform.position = placingPos;
            spawnedObject.transform.localScale = Vector3.one / 4;
        }
    }
}
