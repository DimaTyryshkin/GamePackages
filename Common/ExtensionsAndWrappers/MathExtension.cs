using System;
using UnityEngine;

namespace GamePackages.Core
{
    public static class MathExtension
    {
        /// <summary>
        /// Остаток от деления. Всегда положительный
        /// </summary> 
        public static int Residual(int value, int maxValue)
        {
            int result = value % maxValue;
            if (Math.Sign(result) * Math.Sign(maxValue) < 0)
                result += maxValue;
            return result;
        }


        /// <summary>
        /// Like Sin(x) and ArcSin(x)
        /// </summary>
        public static float ArcLerp(float star, float end, float value)
        {
            float delta = end - star;

            if (Mathf.Approximately(delta, 0))
                return value < star ? 0 : 1;

            float x = value - star;
            return Mathf.Clamp01(x / delta);
        }

        /// <summary>
        /// Переводит value из одного диапозона в другой.
        /// Удобно, чтобы перевести Sin(value) из диапозона [-1,1] в [toA, toB]
        /// </summary>
        public static float Map(float fromA, float fromB, float toA, float toB, float value)
        {
            float mappedValue = MapUnclamped(fromA, fromB, toA, toB, value);
            (float min, float max) = toB >= toA ?
                (toA, toB) :
                (toB, toA);

            return Mathf.Clamp(mappedValue, min, max);
        }

        /// <summary>
        /// Переводит value из одного диапозона в другой.
        /// Удобно, чтобы перевести Sin(value) из диапозона [-1,1] в [toA, toB]
        /// </summary>
        public static float MapUnclamped(float fromA, float fromB, float toA, float toB, float value)
        {
            float fromDelta = fromB - fromA;
            float x = Mathf.Approximately(fromDelta, 0) ?
                (value < fromA ? 0 : 1) :
                (value - fromA) / fromDelta;

            return toA + (toB - toA) * x;
        }

        public static (float, int, Vector3) NearPointOnSegments(Vector3[] segments, Vector3 point)
        {
            float minDistance = float.MaxValue;
            int index = 0;
            Vector3 nearPointOnSegment = segments[0];
            for (int i = 0; i < segments.Length - 1; i++)
            {
                Vector3 p1 = segments[i + 0];
                Vector3 p2 = segments[i + 1];
                Vector3 pointOnSegment = NearPointOnSegment(p1, p2, point);

                float distance = Vector3.Distance(pointOnSegment, point);
                if (distance <= minDistance)
                {
                    minDistance = distance;
                    index = i;
                    nearPointOnSegment = pointOnSegment;
                }
            }

            return (minDistance, index, nearPointOnSegment);
        }

        public static Vector3 NearPointOnSegment(Vector3 p1, Vector3 p2, Vector3 point)
        {
            Vector3 segment = p2 - p1;
            float dot1 = Vector3.Dot(point - p1, segment);
            float dot2 = -Vector3.Dot(point - p2, segment);

            if (dot1 >= 0 && dot2 >= 0)
            {
                float d = GetPlaneD(segment, point);
                //Уравнение плоскости, проходящей через точку point с нормалью segment будет таким: segment*p + d == 0
                Vector3 crossPoint = CrossLineAndPlane(p1, p2, segment, d);
                return crossPoint;
            }
            else
            {
                if (dot1 > dot2)
                    return p2;
                else
                    return p1;
            }
        }

        public static float GetPlaneD(Vector3 planeNormal, Vector3 pointOnPlane)
        {
            return -Vector3.Dot(pointOnPlane, planeNormal);
        }

        public static Vector3 CrossLineAndPlane(Vector3 lineP1, Vector3 lineP2, Vector3 planeNormal, float planeD)
        {
            Vector3 lineDir = lineP2 - lineP1;

            float dot1 = Vector3.Dot(planeNormal, lineP1);
            float dot2 = Vector3.Dot(planeNormal, lineDir);
            float t = -(dot1 + planeD) / dot2;

            Vector3 p = lineP1 + lineDir * t;
            return p;
        }

        public static int PingPong(int t, int length)
        {
            int q = t / length;
            int r = t % length;

            if ((q % 2) == 0)
                return r;
            else
                return length - r;
        }
    }
}