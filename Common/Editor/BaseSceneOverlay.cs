using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePackages.Core
{
    /// <summary>
    /// [Overlay(typeof(SceneView), "Меню объекта")]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSceneOverlay<T> : Overlay, ITransientOverlay where T : MonoBehaviour
    {
        public bool visible
        {
            get
            {
                return Selection.activeGameObject != null &&
                       Selection.activeGameObject.GetComponent<T>() != null;
            }
        }

        // Подписываемся на изменение выделения, чтобы Unity вовремя обновляла видимость
        public override void OnCreated()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            // Принудительно сообщаем Unity, что состояние visible могло измениться
            displayed = visible;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            root.style.width = 160;
            CreatePanelContent(root);
            return root;
        }

        protected T target => Selection.activeGameObject?.GetComponent<T>();
        protected abstract void CreatePanelContent(VisualElement root);
    }
}