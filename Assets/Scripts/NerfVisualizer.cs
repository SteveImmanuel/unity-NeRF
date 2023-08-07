using System;
using UnityEngine;

namespace NeRF
{
    [ExecuteInEditMode]
    public class NerfVisualizer : MonoBehaviour
    {
        public GlobalDataScriptableObject globalData;
        public Boolean useGlobalData;

        // [Header("Visualization Properties")] public float rayLength;
        
        [Header("Camera Properties")]
        public float focalLength;
        
        [Header("Image Plane Properties")]
        public float width ;
        public float height;
        public int nRows;
        public int nCols;
        
        [Header("Transformation Matrix")]
        public Vector3 rotRow1;
        public Vector3 rotRow2;
        public Vector3 rotRow3;
        public Vector3 position;

        [Header("Ray Configurations")] 
        public LayerMask imgPlaneLayerMask;
        public LayerMask sceneLayerMask;
        
        private Camera cam;
        private Ray[,] rays;
        private RaycastHit[,] rayCastHits;
        private GameObject imgPlane;
        
        private void Awake()
        {
            useGlobalData = false;
            cam = gameObject.GetComponent<Camera>();
        }

        private void Start()
        {
            imgPlane = transform.GetChild(0).gameObject;
        }

        private void Update()
        {
            UpdateImgPlane();
            NerfUtils.DrawRays(transform, focalLength, height, width, nRows, nCols, Color.white, imgPlaneLayerMask);
            NerfUtils.DrawImageGrid(transform, focalLength, height, width, nRows, nCols, Color.white);
        }
        
        private void UpdateImgPlane()
        {
            imgPlane.transform.localPosition = new Vector3(0, 0, focalLength - (float) 1e-4);
        }
        

        private float GetFocalLength()
        {
            // Debug.Log(cam.sensorSize);
            return Camera.FieldOfViewToFocalLength(cam.fieldOfView, cam.sensorSize[0]);
        }
    }
}
