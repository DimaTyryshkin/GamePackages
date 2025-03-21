using UnityEngine;

namespace GamePackages.Core
{
    public static class BoundsExtension
    {
        /// <summary>
        /// Create Bounds that encompasses two bounds.
        /// </summary>
        public static Bounds Encompass(Bounds a, Bounds b)
        {
            Vector3 aMin = a.min;
            Vector3 aMax = a.max;

            Vector3 bMin = b.min;
            Vector3 bMax = b.max;

            Vector3 min = Vector3Extension.GetMinComponents(aMin, bMin);
            Vector3 max = Vector3Extension.GetMaxComponents(aMax, bMax);

            return FromMinMax(min, max);
        }

        /// <summary>
        /// Create Bounds that encompasses all points.
        /// </summary>
        public static Bounds Encompass(params Vector3[] points)
        {
            Bounds bounds = new Bounds(points[0], Vector3.zero);

            for (int i = 1; i < points.Length; i++)
            {
                bounds = Encompass(bounds, new Bounds(points[i], Vector3.zero));
            }

            return bounds;
        }

        /// <summary>
        /// Create Bounds that encompasses all bounds.
        /// </summary>
        public static Bounds Encompass(params Bounds[] boundsArray)
        {
            Bounds bounds = boundsArray[0];

            for (int i = 1; i < boundsArray.Length; i++)
                bounds = Encompass(bounds, boundsArray[i]);

            return bounds;
        }

        public static Bounds FromMinMax(Vector3 min, Vector3 max)
        {
            Vector3 size = max - min;
            return new Bounds(min + size * 0.5f, size);
        }

        public static Vector3 GetPointFormNormalizedPoint(this Bounds bounds, Vector3 normalizedPoint)
        {
            Vector3 min = bounds.min;
            Vector3 size = bounds.size;

            return new Vector3
            (
                min.x + size.x * normalizedPoint.x,
                min.y + size.y * normalizedPoint.y,
                min.z + size.z * normalizedPoint.z
            );
        }

        /// <summary>
        /// Creates and returns an enlarged copy of the specified Bounds. The copy is enlarged by the specified amounts.
        /// </summary> 
        public static Bounds Inflate(Bounds a, float value)
        {
            Vector3 extents = a.extents;

            extents.x += Mathf.Sign(extents.x) * value;
            extents.y += Mathf.Sign(extents.y) * value;
            extents.z += Mathf.Sign(extents.z) * value;

            return new Bounds(a.center, extents * 2);
        }

        public static Bounds ToFlatXZ(Bounds a)
        {
            Vector3 extents = a.extents;
            extents.y = 0;
            return new Bounds(a.center, extents * 2);
        }
    }
}