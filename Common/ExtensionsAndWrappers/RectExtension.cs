
using System.Collections.Generic;
using UnityEngine;


namespace GamePackages.Core
{
    public static class RectExtension
    {
        public static Rect GetRect(Vector2 p1, Vector2 p2)
        {
            return new Rect(
                Mathf.Min(p1.x, p2.x),
                Mathf.Min(p1.y, p2.y),
                Mathf.Abs(p2.x - p1.x),
                Mathf.Abs(p2.y - p1.y));
        }

        public static Rect ScaleSize(this Rect rect, float scale)
        {
            return rect.ScaleSize(scale, rect.center);
        }

        public static Rect ScaleSize(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;

            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;

            result.x += pivotPoint.x;
            result.y += pivotPoint.y;

            return result;
        }

        public static Rect SplitGuiRectOnLines(this Rect rect, int linesAmount, int lineIndex)
        {
            if (linesAmount == 1)
                return rect;

            Vector2 pos = rect.position;
            Vector2 size = rect.size;

            float lineHeight = size.y / linesAmount;

            return new Rect(pos.x, pos.y + lineHeight * lineIndex, size.x, lineHeight);
        }

        public static IEnumerable<Vector2Int> ForEach(this RectInt rect)
        {
            var min = rect.min;
            var max = rect.max;
            for (int x = min.x; x < max.x; x++)
            {
                for (int y = min.y; y < max.y; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        public static Vector2 RandomPoint(this Rect rect)
        {
            Vector2 p1 = rect.min;
            Vector3 p2 = rect.max;

            return new Vector3(
                Random.Range(p1.x, p2.x),
                Random.Range(p1.y, p2.y));
        }
    }
}