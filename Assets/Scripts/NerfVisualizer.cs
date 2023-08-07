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
        public float rayIterator;
        public RayType rayType;
        [Range(1, 100)]
        public int maxRowGrid;
        public LayerMask objLayerMask;
        public Transform directionalLight;
        public Color rayColor;
        public Color imageGridColor;

        private Camera cam;
        
        private void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            ApplyCameraConfig();
            (float, float) dim = CalculateImageDimension();

            if (showGrid)
            {
                NerfUtils.DrawImageGrid(transform, cam.nearClipPlane, dim.Item1, dim.Item2, maxRowGrid, imageGridColor);
            }

            Ray[,] rays = NerfUtils.CalculateRays(transform, cam.nearClipPlane, dim.Item1, dim.Item2, maxRowGrid);
            if (showRays)
            {
                if (rayType == RayType.Image)
                {
                    NerfUtils.DrawRaysImagePlane(transform, cam.nearClipPlane, rays, rayColor, rayIterator);
                }
                else
                {
                    NerfUtils.DrawRaysScene(rays, rayColor, rayIterator, objLayerMask, cam.farClipPlane);
                }
            }

            Vector3 lightDir = GetLightDirection();
            Color[,] colors = NerfUtils.GetRenderedColor(rays, lightDir, Color.black, rayIterator, objLayerMask, cam.farClipPlane);
            NerfUtils.LogMatrix(colors);
        }

        private Vector3 GetLightDirection()
        {
            float[,] rotMat = NerfUtils.EulerToMatrix(directionalLight.eulerAngles, "zxy");
            return NerfUtils.CalculateDirection(rotMat, new Vector3(0, 0, 1));
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
