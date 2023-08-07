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
        [Range(0f, 1f)]
        public float rayDensity;
        [Range(0f, 1f)]
        public float rayIterator;
        public int maxRowGrid;

        private Camera cam;
        private Ray[,] rays;
        private RaycastHit[,] rayCastHits;
        
        private void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            ApplyCameraConfig();
            (float, float) dim = CalculateImageDimension();
            NerfUtils.DrawImageGrid(transform, cam.nearClipPlane, dim.Item1, dim.Item2, rayDensity, maxRowGrid, Color.yellow);
        }

        private void ApplyCameraConfig()
        {
            cam.nearClipPlane = nearClippingDistance;
            cam.focalLength = focalLength;
            cam.sensorSize = new Vector2(sensorX, sensorY);
        }
        
        private (float, float) CalculateImageDimension()
        {
            float height = this.cam.nearClipPlane * this.cam.sensorSize[1] / this.cam.focalLength;
            float width = cam.nearClipPlane * cam.sensorSize[0] / cam.focalLength;
            return (height, width);
        }
    }
}
