using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    public static class GUIExtension
    {
        public static void DrawSprite(Rect rect, Sprite sprite)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Rect c = sprite.textureRect;
                var tex = sprite.texture;
                c.xMin /= tex.width;
                c.xMax /= tex.width;
                c.yMin /= tex.height;
                c.yMax /= tex.height;
                GUI.DrawTextureWithTexCoords(rect, tex, c);
                //ScaleMode.StretchToFill
            }
        }
    }

    public static class GUILayoutExtension
    {
        public static void DrawSprite(Sprite sprite, float maxSize)
        {
            Rect r = sprite.rect;
            Vector2Int size = GetTextureSize(r.width, r.height, maxSize, maxSize, 0, 0);
            int w = size.x;
            int h = size.y;
            Rect rect = GUILayoutUtility.GetRect(w, h, GUILayout.Width(w), GUILayout.Height(h));
            GUIExtension.DrawSprite(rect, sprite);
        }

        public static Vector2Int GetTextureSize(
            float width, float height,
            float maxWidth, float maxHeight,
            float minWidth, float minHeight)
        {
            if (maxWidth > 0 && minWidth > 0)
                Assert.IsTrue(minWidth <= maxHeight);

            if (maxHeight > 0 && minHeight > 0)
                Assert.IsTrue(minHeight <= maxHeight);

            if (maxWidth > 0 && (float)width > maxWidth)
            {
                float k = (float)width / maxWidth;
                width /= k;
                height /= k;
            }

            if (maxHeight > 0 && (float)height > maxHeight)
            {
                float k = (float)height / maxHeight;
                width /= k;
                height /= k;
            }

            if (minWidth > 0 && (float)width < minWidth)
            {
                float k = (float)width / minWidth;
                width /= k;
                height /= k;
            }

            if (minHeight > 0 && (float)height < minHeight)
            {
                float k = (float)height / minHeight;
                width /= k;
                height /= k;
            }

            //return new Vector2Int(
            //    Mathf.Min((int)maxWidth, (int)width),
            //    Mathf.Min((int)maxHeight, (int)height));

            return new Vector2Int((int)width, (int)height);
        }
    }
}