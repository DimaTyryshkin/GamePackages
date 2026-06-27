using System.Collections.Generic;
using GamePackages.Core.Validation;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Core
{
    public class GizmosDrawer : MonoBehaviour
    {
        [SerializeField, IsntNull] DebugMarker prefab;
        public Color color = Color.white;
        List<GizmosFigure> figures;

        static GizmosDrawer inst;


        public static GizmosDrawer Inst
        {
            get
            {
                if (!inst)
                    inst = FindAnyObjectByType<GizmosDrawer>();

                return inst;
            }
        }

        List<GizmosFigure> Figures
        {
            get
            {
                if (figures == null)
                    figures = new List<GizmosFigure>(128);

                return figures;
            }
        }

        void OnDrawGizmos()
        {
            if (figures != null)
            {
                Color oldColor = Gizmos.color;
                try
                {
                    foreach (var f in figures)
                    {
                        Gizmos.color = f.color;
                        f.Draw();
                    }

                    figures.RemoveAll(f => f.timeDeath != 0 && Time.realtimeSinceStartup > f.timeDeath);
                }
                finally
                {
                    Gizmos.color = oldColor;
                }
            }
        }

        void AddFigure(GizmosFigure f)
        {
            Figures.Add(f);
            f.color = color;
        }

        public void ClearFigures()
        {
            figures?.Clear();
        }

        public DebugMarker GetMarker(Vector3 pos)
        {
            DebugMarker marker = transform.InstantiateAsChild(prefab);
            marker.gameObject.hideFlags = HideFlags.DontSave;
            marker.transform.position = pos;
            marker.gameObject.SetActive(true);

            return marker;
        }

        public GizmosArrow AddArrow(Vector3 p1, Vector3 p2, bool isXY = true)
        {
            GizmosArrow figure = new GizmosArrow(p1, p2, isXY);
            AddFigure(figure);
            return figure;
        }

        public GizmosLine AddLine(Vector3 p1, Vector3 p2)
        {
            var figure = new GizmosLine(p1, p2);
            AddFigure(figure);
            return figure;
        }


        public GizmosText AddText<T>(Vector3 position, T obj) => AddText(position, obj.ToString());

        public GizmosText AddText(Vector3 position, string text)
        {
            var figure = new GizmosText(position, text);
            AddFigure(figure);
            return figure;
        }

        public abstract class GizmosFigure
        {
            public float timeDeath;
            public Color color;

            public float Duration
            {
                set => timeDeath = Time.realtimeSinceStartup + value;
            }

            public abstract void Draw();

            public GizmosFigure SetDuration(float duration)
            {
                timeDeath = Time.realtimeSinceStartup + duration;
                return this;
            }

            public GizmosFigure SetColor(Color color)
            {
                this.color = color;
                return this;
            }
        }

        public class GizmosArrow : GizmosFigure
        {
            public Vector3 p1;
            public Vector3 p2;
            public bool isXY;

            public GizmosArrow(Vector3 p1, Vector3 p2, bool isXY)
            {
                this.p1 = p1;
                this.p2 = p2;
                this.isXY = isXY;
            }

            public override void Draw()
            {
                Gizmos.color = color;
                if (isXY)
                    GizmosExtension.DrawArrowXY(p1, p2);
                else
                    GizmosExtension.DrawArrowXZ(p1, p2);
            }
        }

        public class GizmosLine : GizmosFigure
        {
            public Vector3 p1;
            public Vector3 p2;

            public GizmosLine(Vector3 p1, Vector3 p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }

            public override void Draw()
            {
                Gizmos.DrawLine(p1, p2);
            }
        }

        public class GizmosText : GizmosFigure
        {
            public Vector3 position;
            public string text;

            public GizmosText(Vector3 p1, string text)
            {
                this.position = p1;
                this.text = text;
            }

            public override void Draw()
            {
#if UNITY_EDITOR
                Handles.Label(position, text);
#endif
            }
        }
    }
}