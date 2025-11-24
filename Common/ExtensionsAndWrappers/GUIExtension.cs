using UnityEngine;

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
            Vector2Int size = GetTextureSize(r.width, r.height, maxSize, maxSize);
            int w = size.x;
            int h = size.y;
            Rect rect = GUILayoutUtility.GetRect(w, h, GUILayout.Width(w), GUILayout.Height(h));
            GUIExtension.DrawSprite(rect, sprite);
        }

        public static Vector2Int GetTextureSize(float width, float height, float maxWidth, float maxHeight)
        {
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

            //return new Vector2Int(
            //    Mathf.Min((int)maxWidth, (int)width),
            //    Mathf.Min((int)maxHeight, (int)height));

            return new Vector2Int((int)width, (int)height);
        }
    }
}