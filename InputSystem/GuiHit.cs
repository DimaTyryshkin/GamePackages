using GamePackages.Core;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GamePackages.InputSystem
{
    public class GuiHit : MonoBehaviour
    {
        [SerializeField]
        GraphicRaycaster[] graphicRaycasters;

        List<RaycastResult> objectUnderPointer = new List<RaycastResult>();
        bool isGuiUnderPointer = false;
        bool valueCalculated = true; //Пропускаем первый кадр

        public bool IsGuiUnderPointer
        {
            get
            {
                if (valueCalculated)
                    return isGuiUnderPointer;

                isGuiUnderPointer = HitGraphic();
                valueCalculated = true;
                return isGuiUnderPointer;
            }
        }

        public static Vector2 GetWorldPoint(Transform transform, Camera gameCamera)
        {
            Canvas canvas = transform.GetComponentInParent<Canvas>();
            if (canvas)
            {
                Camera cam = null;
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                    cam = canvas.worldCamera;

                Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, transform.position);
                return gameCamera.ScreenPointToWorldPointOnPlane(screenPoint, Plaine.XY);
            }
            else
            {
                return transform.position;
            }
        }

        bool HitGraphic()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            PointerEventData pointEvent = new PointerEventData(null);
            pointEvent.position = mousePos;

            foreach (var raycaster in graphicRaycasters)
            {
                objectUnderPointer.Clear();
                raycaster.Raycast(pointEvent, objectUnderPointer);
                if (objectUnderPointer.Count > 0)
                    return true;
            }

            return false;
        }

        void LateUpdate()
        {
            valueCalculated = false;
        }

        [SerializeField] Camera gameCamera;
        [SerializeField] Transform rectGui;
        [SerializeField] Transform objectToReplace;

        [Button]
        void TestGetWorldPoint()
        {
            objectToReplace.position = GetWorldPoint(rectGui, gameCamera);
        }
    }
}