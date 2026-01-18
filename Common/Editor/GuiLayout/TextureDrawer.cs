using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core.Editor
{
    public class TextureDrawer
    {
        public int scale = 1;
        public Vector2 offset;
        bool isDrag;
        int maxScale = 16;
        Rect texCoords;
        Rect viewRect;
        Texture2D texture;

        public TextureDrawer(Texture2D texture)
        {
            Assert.IsNotNull(texture);
            this.texture = texture;
        }

        public void Draw(
            float maxWidth, float maxHeight,
            float minWidth, float minHeight)
        {
            GUILayout.BeginVertical();
            {
                //Assert.IsNull(texture);
                //Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, asset.maxWidthOnGui, 0);
                Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, maxWidth, maxHeight, minWidth, minHeight);
                float defaultScale = size.x / (float)texture.width;

                GUILayoutOption width = GUILayout.Width(size.x);
                viewRect = GUILayoutUtility.GetRect(0, 0, width, GUILayout.Height(size.y));

                CalculateTexCoords();
                GUI.DrawTextureWithTexCoords(viewRect, texture, texCoords);

                Vector2 mousePos = Event.current.mousePosition;
                if (viewRect.Contains(mousePos))
                {
                    bool isClick = false;
                    if (Event.current.type == EventType.MouseDown)
                    {
                        isDrag = false;
                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {
                        isDrag = true;
                        Vector2 delta = Event.current.delta;
                        delta.x *= -1f / (texture.width * scale * defaultScale);
                        delta.y *= 1f / (texture.height * scale * defaultScale);
                        offset += delta;

                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        if (!isDrag)
                        {
                            isClick = true;
                        }

                        isDrag = false;
                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        float scroll = Mathf.Sign(Event.current.delta.y);
                        if (!Mathf.Approximately(scroll, 0))
                        {
                            Vector2 mouseNormilizedPosOld = ScreenPointToNormilizedTextCoord(mousePos);
                            scale = Mathf.Clamp(scale - (int)scroll, 1, maxScale);
                            CalculateTexCoords();
                            Vector2 mouseNormilizedPosNew = ScreenPointToNormilizedTextCoord(mousePos);

                            Vector2 delta = mouseNormilizedPosOld - mouseNormilizedPosNew;
                            offset += delta;

                            Event.current.Use();
                        }
                    }

                    if (isClick)
                    {
                        Vector2Int mouseTextCoord = ScreenPointToTextCoord(mousePos);
                        texture.SetPixel(mouseTextCoord.x, mouseTextCoord.y, Color.magenta);
                        texture.Apply();
                    }
                }
            }
            GUILayout.EndVertical();
        }

        Vector2 ScreenPointToNormilizedTextCoord(Vector2 point)
        {
            Vector2 normPos = Rect.PointToNormalized(viewRect, point);
            Vector2 add = new Vector2(normPos.x * texCoords.width, (1f - normPos.y) * texCoords.width);
            Vector2 pointNormilizedTextCoord = offset + add;
            return pointNormilizedTextCoord;
        }

        Vector2Int ScreenPointToTextCoord(Vector2 point)
        {
            Vector2 pointNormilizedTextCoord = ScreenPointToNormilizedTextCoord(point);

            Vector2Int pointTextCoord = new Vector2Int(
                Mathf.RoundToInt(texture.width * pointNormilizedTextCoord.x - 0.5f),
                Mathf.RoundToInt(texture.height * pointNormilizedTextCoord.y - 0.5f));

            return pointTextCoord;
        }

        void CalculateTexCoords()
        {
            float textCoordsSize = 1f / scale;
            texCoords = new Rect(offset.x, offset.y, textCoordsSize, textCoordsSize);
        }

        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
        }
    }

}
