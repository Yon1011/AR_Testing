using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace Clayno.AR
{
    public class ObjectSpawner : MonoBehaviour
    {
        Camera m_CameraToFace;

        public Camera cameraToFace
        {
            get
            {
                EnsureFacingCamera();
                return m_CameraToFace;
            }
            set => m_CameraToFace = value;
        }

        [SerializeField]
        GameObject m_ObjectPrefab;

        [SerializeField]
        bool m_OnlySpawnInView = true;

        [SerializeField]
        float m_ViewportPeriphery = 0.15f;

        [SerializeField]
        float m_SpawnAngleRange = 45f;
        public event Action<GameObject> objectSpawned;

        void Awake()
        {
            EnsureFacingCamera();
        }

        void EnsureFacingCamera()
        {
            if (m_CameraToFace == null)
                m_CameraToFace = Camera.main;
        }

        public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
        {
            if (m_OnlySpawnInView)
            {
                var inViewMin = m_ViewportPeriphery;
                var inViewMax = 1f - m_ViewportPeriphery;
                var pointInViewportSpace = cameraToFace.WorldToViewportPoint(spawnPoint);
                if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
                {
                    return false;
                }
            }

            var newObject = Instantiate(m_ObjectPrefab, transform);
            newObject.transform.position = spawnPoint;
            EnsureFacingCamera();

            var facePosition = m_CameraToFace.transform.position;
            var forward = facePosition - spawnPoint;
            BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
            newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);
            objectSpawned?.Invoke(newObject);
            return true;
        }
    }
}
