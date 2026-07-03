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
    }
}