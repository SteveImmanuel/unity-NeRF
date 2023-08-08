using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeRF
{
    [RequireComponent(typeof(NerfVisualizer))]
    public class NerfRenderer : MonoBehaviour
    {
        public Transform directionalLight;
        public Color backgroundColor;
        private NerfVisualizer nerf;

        private void Awake()
        {
            nerf = GetComponent<NerfVisualizer>();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {            
            Color[,] colors = GetPixelColors();
            Texture2D texture = new Texture2D(colors.GetLength(1), colors.GetLength(0), TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            for (int i = 0; i < colors.GetLength(1); i++)
            {
                for (int j = 0; j < colors.GetLength(0); j++)
                {
                    texture.SetPixel(i, j, colors[colors.GetLength(0) - 1 - j, i]);
                }
            }
            texture.Apply();
            
            Graphics.Blit(texture, destination);
            
        }

        private Color[,] GetPixelColors()
        {
            (float, float) dim = NerfUtils.CalculateImageDimension(nerf.cam.focalLength, nerf.cam.nearClipPlane, nerf.cam.sensorSize);
            Ray[,] rays = NerfUtils.CalculateRays(transform, nerf.cam.nearClipPlane, dim.Item1, dim.Item2, nerf.maxRowGrid);
            Color[,] colors = NerfUtils.GetRenderedColor(rays, GetLightDirection(), backgroundColor, nerf.rayConfig, nerf.objLayerMask, nerf.cam.farClipPlane);
            
            return colors;
        }
        
        private Vector3 GetLightDirection()
        {
            float[,] rotMat = NerfUtils.EulerToMatrix(directionalLight.eulerAngles, "zxy");
            return NerfUtils.CalculateDirection(rotMat, new Vector3(0, 0, 1)); // can be replaced with simple directionalLight.forward
        }
    }
}
