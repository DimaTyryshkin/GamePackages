using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        [Obsolete]
        public bool IsGuiUnderPointer
        {
            get
            {
                HitGraphic(Input.mousePosition);
                return isGuiUnderPointer;
            }
        }

        public bool IsGuiUnderMouse(Vector2 mousePos)
        {
            HitGraphic(mousePos);
            return isGuiUnderPointer;
        }

        void HitGraphic(Vector2 mousePos)
        {
            if (valueCalculated == false)
            {
                isGuiUnderPointer = false;
                valueCalculated = true;

                PointerEventData pointEvent = new PointerEventData(null);
                pointEvent.position = mousePos;

                foreach (var raycaster in graphicRaycasters)
                {
                    objectUnderPointer.Clear();
                    raycaster.Raycast(pointEvent, objectUnderPointer);
                    isGuiUnderPointer = isGuiUnderPointer || objectUnderPointer.Count > 0;
                }
            }
        }

        void LateUpdate()
        {
            valueCalculated = false;
        }
    }
}