using GamePackages.Core;
using UnityEngine;

namespace GamePackages.Tools.Alignment
{
    public static class Alignment
    {
        public static void AlignmentAboveParent(Transform t)
        {
            if (t.parent)
            {
                Bounds bounds = t.GetTotalRendererBounds();
                t.position += (t.parent.position - bounds.center);
                t.position += new Vector3(0, bounds.size.y * 0.5f, 0);
            }
        }
    }
}