using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core.Editor
{
    public class TextureDrawer
    {
        int scale = 1;
        bool isDrag;
        int maxScale = 16;
        Vector2 offset;
        Rect texCoords;
        Rect viewRect;
        Texture2D texture;

        public TextureDrawer(Texture2D texture)
        {
            Assert.IsNotNull(texture);
            this.texture = texture;
        }

        public void Draw(int maxWidth = 0, int maxHeight = 0)
        {
            GUILayout.BeginVertical();
            {
                //Assert.IsNull(texture);
                //Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, asset.maxWidthOnGui, 0);
                Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, maxWidth, maxHeight);
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
                        delta.x *= -1f / (texture.width * scale);
                        delta.y *= 1f / (texture.height * scale);
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


                GUILayout.BeginHorizontal();
                {
                    scale = EditorGUILayout.IntSlider(scale, 1, maxScale, width);
                }
                GUILayout.EndHorizontal();
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
    }

}
