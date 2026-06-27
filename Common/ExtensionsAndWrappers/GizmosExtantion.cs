#if UNITY_EDITOR && !NO_EDITOR_CODE
using System;
using UnityEditor;
using UnityEngine;

namespace GamePackages.Core
{
    public static class GizmosExtension
    {
        static Quaternion xyRight = Quaternion.Euler(0, 0, +20);
        static Quaternion xyLeft = Quaternion.Euler(0, 0, -20);

        static Quaternion xzRight = Quaternion.Euler(0, +20, 0);
        static Quaternion xzLeft = Quaternion.Euler(0, -20, 0);

        public static void DrawArrowXY(Vector3 from, Vector3 to, float size = 0.25f)
        {
            DrawArrow(from, to, size, xyRight, xyLeft);
        }

        public static void DrawArrowXZ(Vector3 from, Vector3 to, float size = 0.25f)
        {
            DrawArrow(from, to, size, xzRight, xzLeft);
        }

        static void DrawArrow(Vector3 from, Vector3 to, float size, Quaternion right, Quaternion left)
        {
            Vector3 direction = to - from;
            Gizmos.DrawRay(from, direction);

            Vector3 arrow = direction * (-size);
            Gizmos.DrawRay(to, right * arrow);
            Gizmos.DrawRay(to, left * arrow);
        }

        public static void DrawRect(Rect rect)
        {
            Vector2 p1 = rect.min;
            Vector2 p2 = p1 + new Vector2(0, rect.height);
            Vector2 p3 = rect.max;
            Vector2 p4 = p1 + new Vector2(rect.width, 0);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p1, p4);
        }

        public static void DrawRectXZ(Rect rect)
        {
            Vector3 p1 = new Vector3(rect.min.x, 0, rect.min.y);
            Vector3 p2 = p1 + new Vector3(0, 0, rect.height);
            Vector3 p3 = new Vector3(rect.max.x, 0, rect.max.y);
            Vector3 p4 = p1 + new Vector3(rect.width, 0, 0);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p1, p4);
        }

        public static void DrawBounds(Bounds bounds)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public static void DrawLines(Vector3[] points, float pointRadius = 0) => DrawLines(points, p => p, pointRadius);
        public static void DrawLines(Vector2[] points, float pointRadius = 0) => DrawLines(points, p => (Vector3)p, pointRadius);
        public static void DrawLines(Transform[] points, float pointRadius = 0) => DrawLines2(points, p => p.position, pointRadius);


        public static void DrawLines<T>(T[] points, Func<T, Vector3> getPos, float pointRadius = 0)
           where T : struct
        {
            if (points == null)
                return;

            for (int i = 1; i < points.Length; i++)
            {
                Gizmos.DrawLine(getPos(points[i - 1]), getPos(points[i]));
            }

            if (pointRadius > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Handles.DrawWireDisc(getPos(points[i]), Vector3.up, pointRadius);
                }
            }
        }

        public static void DrawLines2<T>(T[] points, Func<T, Vector3> getPos, float pointRadius = 0)
            where T : UnityEngine.Object
        {
            if (points == null)
                return;

            for (int i = 1; i < points.Length; i++)
            {
                if (!points[i - 1] || !points[i])
                    return;

                Gizmos.DrawLine(getPos(points[i - 1]), getPos(points[i]));
            }

            if (pointRadius > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Handles.DrawWireDisc(getPos(points[i]), Vector3.up, pointRadius);
                }
            }
        }
    }
}
#endif