using System;
using UnityEngine;

namespace NeRF
{
    [ExecuteInEditMode]
    public class NerfVisualizer : MonoBehaviour
    {
        [Header("Camera Configurations")] 
        public float focalLength;
        public float sensorX;
        public float sensorY;
        public float nearClippingDistance;
        
        [Header("Transformation Matrix")]
        public Vector3 rotRow1;
        public Vector3 rotRow2;
        public Vector3 rotRow3;
        public Vector3 position;

        [Header("Visualizations Configurations")]
        public bool showGrid;
        public bool showRays;
        [SerializeField] public RayConfig rayConfig;
        [Range(1, 256)]
        public int maxRowGrid;
        public LayerMask objLayerMask;
        public Color rayColor;
        public Color imageGridColor;

        [HideInInspector] public Camera cam;
        
        private void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            ApplyCameraConfig();
            (float, float) dim = NerfUtils.CalculateImageDimension(cam.focalLength, cam.nearClipPlane, cam.sensorSize);

            if (showGrid)
            {
                NerfUtils.DrawImageGrid(transform, cam.nearClipPlane, dim.Item1, dim.Item2, maxRowGrid, imageGridColor);
            }

            if (showRays)
            {
                Ray[,] rays = NerfUtils.CalculateRays(transform, cam.nearClipPlane, dim.Item1, dim.Item2, maxRowGrid);
                NerfUtils.DrawRays(transform, cam.nearClipPlane, rays, rayColor, rayConfig, objLayerMask, cam.farClipPlane);
            }
        }

        

        private void ApplyCameraConfig()
        {
            cam.nearClipPlane = nearClippingDistance;
            cam.focalLength = focalLength;
            cam.sensorSize = new Vector2(sensorX, sensorY);
        }
    }
}
