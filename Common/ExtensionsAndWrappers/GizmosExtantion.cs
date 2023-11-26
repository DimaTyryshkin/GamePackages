using UnityEngine;

namespace GamePackages.Core
{
    public static class GizmosExtension
    {
       static  Quaternion xyRight = Quaternion.Euler(0, 0, +20);
       static Quaternion xyLeft = Quaternion.Euler(0, 0, -20);
       
       static  Quaternion xzRight = Quaternion.Euler(0, +20, 0);
       static Quaternion xzLeft = Quaternion.Euler(0, -20, 0);

        public static void DrawArrowXY(Vector3 from, Vector3 to, float size = 0.25f)
        {
            DrawArrow(from, to, size, xyRight, xyLeft);
        }
        
        public static void DrawArrowXZ(Vector3 from, Vector3 to, float size = 0.25f)
        {
            DrawArrow(from, to, size, xzRight, xzLeft);
        }
        
        static void DrawArrow(Vector3 from, Vector3 to, float size , Quaternion right, Quaternion left)
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
            Vector3 p2 = p1 + new Vector3(0, 0,rect.height);
            Vector3 p3 = new Vector3(rect.max.x, 0, rect.max.y);
            Vector3 p4 = p1 + new Vector3(rect.width,0, 0);
 
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p1, p4);
        }  
        
        public static void DrawBounds(Bounds bounds)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public static void DrawLines(Vector3[] points, float pointRadius = 0)
        {
            for (int i = 1; i < points.Length; i++)
            {
                Gizmos.DrawLine(points[i - 1], points[i]);
            }

            if (pointRadius > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Gizmos.DrawWireSphere(points[i], pointRadius);
                }
            }
        }
    }
}