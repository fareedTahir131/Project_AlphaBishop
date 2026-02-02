using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(ARPlaneManager))]
    public class PlaceOnPlane : MonoBehaviour
    {
        [Header("Placement")]
        [SerializeField] private GameObject m_PlacedPrefab;
        [SerializeField] private GameObject placementIndicator;

        [Header("UI")]
        [SerializeField] private Canvas coachingCanvas;
        [SerializeField] private GameObject scanningUI;     // Screen 1
        [SerializeField] private GameObject tapToPlaceUI;   // Screen 2
        [SerializeField] private bool showPlanes = true; // Enable after placement
        [SerializeField] private bool AllowPrefabReposition = true;

        [Header("Events")]
        public UnityEvent onContentPlaced;

        public GameObject spawnedObject { get; private set; }

        private ARRaycastManager raycastManager;
        private ARPlaneManager planeManager;

        private static readonly List<ARRaycastHit> hits = new();

        private bool planeDetected;
        private bool placementPoseValid;

        private Pose placementPose;

        void Awake()
        {
            raycastManager = GetComponent<ARRaycastManager>();
            planeManager = GetComponent<ARPlaneManager>();

            placementIndicator.SetActive(false);

            scanningUI.SetActive(true);
            tapToPlaceUI.SetActive(false);
            UpdatePlaneVisibility();
        }

        void OnEnable()
        {
            planeManager.planesChanged += OnPlanesChanged;
        }

        void OnDisable()
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            if (!planeDetected && planeManager.trackables.count > 0)
            {
                planeDetected = true;

                scanningUI.SetActive(false);
                tapToPlaceUI.SetActive(true);

                placementIndicator.SetActive(true);
                UpdatePlaneVisibility();
            }
        }

        void Update()
        {
            // if (spawnedObject != null)
            //     return;

            UpdatePlacementIndicator();

            // if (!placementPoseValid)
            //     return;

            if (TryGetTouch(out _))
            {
                if(spawnedObject==null){
                    PlaceObject();
                }
                else if(AllowPrefabReposition){
                    spawnedObject.transform.SetPositionAndRotation(
                        placementIndicator.transform.position,
                        placementIndicator.transform.rotation);
                }
                else{
                    // Do nothing
                }
            }
        }

        // -------------------------------------

        private void UpdatePlacementIndicator()
        {
            Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);

            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                placementPoseValid = true;
                placementPose = hits[0].pose;

                if (!placementIndicator.activeSelf)
                    placementIndicator.SetActive(true);

                placementIndicator.transform.SetPositionAndRotation(
                    placementPose.position,
                    placementPose.rotation);
                    
            }
            else
            {
                placementPoseValid = false;

                if (placementIndicator.activeSelf)
                    placementIndicator.SetActive(false);
            }
        }


        // -------------------------------------

        bool TryGetTouch(out Vector2 pos)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                pos = Input.mousePosition;
                return true;
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                pos = Input.GetTouch(0).position;
                return true;
            }
#endif

            pos = default;
            return false;
        }

        // -------------------------------------

        private void PlaceObject()
        {
            spawnedObject = Instantiate(
                m_PlacedPrefab,
                placementIndicator.transform.position,
                placementIndicator.transform.rotation);

            DisableAllPlanes();

            coachingCanvas.gameObject.SetActive(false);

            placementIndicator.SetActive(AllowPrefabReposition);

            onContentPlaced?.Invoke();

            Handheld.Vibrate();

            planeManager.enabled = false;
        }

        // -------------------------------------

        private void DisableAllPlanes()
        {
            foreach (var plane in planeManager.trackables)
                plane.gameObject.SetActive(false);
        }
        void UpdatePlaneVisibility()
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(showPlanes);
            }
        }
    }
}
