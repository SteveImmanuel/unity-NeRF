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
        
        public static float[,] EulerToMatrix(Vector3 eulerAngles)
        {
            float[,] result = new float[3, 3];

            float[,] rx = AngleToMatrix(eulerAngles.x, 'x');
            float[,] ry = AngleToMatrix(eulerAngles.x, 'y');
            float[,] rz = AngleToMatrix(eulerAngles.x, 'z');

            return MatMul(rz, MatMul(ry, rx));
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
    }
}
