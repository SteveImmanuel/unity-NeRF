using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace NeRF
{
    public static class NerfUtils
    {
        public static float[,] MatMul(float[,] mat1, float[,] mat2)
        {
            Assert.AreEqual(mat1.GetLength(1), mat2.GetLength(0));
            float[,] result = new float[mat1.GetLength(0), mat2.GetLength(1)];
            for (int i = 0; i < mat1.GetLength(0); i++)
            {
                for (int j = 0; j < mat2.GetLength(1); j++)
                {
                    float sum = 0;
                    for (int k = 0; k < mat1.GetLength(1); k++)
                    {
                        sum += mat1[i, k] * mat2[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static float[,] AngleToMatrix(float angle, char axis)
        {
            if (axis == 'x')
            {
                return new float[3,3]
                {
                    {1, 0, 0},
                    {0, Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad)},
                    {0, Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)}
                };
            }
            if (axis == 'y')
            {
                return new float[3,3]
                {
                    {Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)},
                    {0, 1, 0},
                    {-Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)}
                };
            }
            if (axis == 'z')
            {
                return new float[3,3]
                {
                    {Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad), 0},
                    {Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0},
                    {0, 0, 1}
                };
            }

            throw new ArgumentException($"Invalid axis value '{axis}'. Expected 'x', 'y', or 'z'.");
        }
        
        public static float[,] EulerToMatrix(Vector3 eulerAngles, string order = "xyz")
        {
            Dictionary<char, float[,]> rotDict = new Dictionary<char, float[,]>
            {
                {'x', AngleToMatrix(eulerAngles.x, 'x')},
                {'y', AngleToMatrix(eulerAngles.y, 'y')},
                {'z', AngleToMatrix(eulerAngles.z, 'z')},
            };

            return MatMul(rotDict[order[2]], MatMul(rotDict[order[1]], rotDict[order[0]]));
        }

        public static void LogMatrix<T>(T[,] mat)
        {
            StringBuilder log = new StringBuilder();
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (j == 0)
                    {
                        log.Append(mat[i, j]);
                    }
                    else
                    {
                        log.Append(", " + mat[i, j]);
                    }
                }
                log.Append('\n');
            }
            Debug.Log(log.ToString());
        }

        public static void DrawImageGrid(Transform transform, float focalLength, float height, float width, int nRows, int nCols, Color color)
        {
            Vector3 leftUpperCorner = transform.position + focalLength * transform.forward + 0.5f * (transform.up * height - transform.right * width);

            Vector3 rowDelta = -transform.up * height / nRows;
            for (int i = 0; i < nRows + 1; i++)
            {
                Debug.DrawLine(leftUpperCorner + i * rowDelta, leftUpperCorner + i * rowDelta + transform.right * width, color);
            }
            
            Vector3 colDelta = transform.right * width / nCols;
            for (int i = 0; i < nCols + 1; i++)
            {
                Debug.DrawLine(leftUpperCorner + i * colDelta, leftUpperCorner + i * colDelta - transform.up * height, color);
            }
        }
        
        public static (Ray[,], RaycastHit[,]) CalculateRays(Transform transform, float focalLength, float height, float width, int nRows, int nCols, Color color, LayerMask imgPlaneLayerMask)
        {
            Ray[,] rays = new Ray[nRows, nCols];
            RaycastHit[,] rayCastHits = new RaycastHit[nRows, nCols];
            float[,] rotMat = EulerToMatrix(transform.eulerAngles, "zxy"); // rotation order is confirmed from experiments
            
            for (int i = 0; i < rays.GetLength(0); i++)
            {
                for (int j = 0; j < rays.GetLength(1); j++)
                {
                    float[,] rawDirection = new float[3,1]
                    {
                        {(j - (nCols - 1) * .5f) * (width / nCols) / focalLength},
                        {(-(i - (nRows - 1) * .5f)) * (height / nRows) / focalLength}, 
                        {1}
                    };
                    float[,] transformedDirection = NerfUtils.MatMul(rotMat, rawDirection);
                    rays[i, j] = new Ray( 
                        transform.position,
                        new Vector3(transformedDirection[0, 0], transformedDirection[1, 0], transformedDirection[2, 0])
                    );
                    rayCastHits[i, j] = new RaycastHit();
                    Physics.Raycast(rays[i, j], out rayCastHits[i, j], 10, imgPlaneLayerMask);
                }
            }

            return (rays, rayCastHits);
        }
        
        public static void DrawRays(Transform transform, float focalLength, float height, float width, int nRows, int nCols, Color color, LayerMask imgPlaneLayerMask)
        {
            (Ray[,], RaycastHit[,]) rayInfo = CalculateRays(transform, focalLength, height, width, nRows, nCols, color, imgPlaneLayerMask);
            for (int i = 0; i < rayInfo.Item1.GetLength(0); i++)
            {
                for (int j = 0; j < rayInfo.Item1.GetLength(1); j++)
                {
                    Debug.DrawRay(rayInfo.Item1[i, j].origin, rayInfo.Item2[i, j].point - rayInfo.Item1[i, j].origin, Color.green);
                }
            }
        }
    }
}
