using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GamePackages.Core
{

    public class ButtonsGroup : MonoBehaviour
    {
        [SerializeField, IsntNull] Button[] buttons;

        void Reset()
        {
            List<Button> buttonsList = new List<Button>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var button = transform.GetChild(i).GetComponent<Button>();
                if (button)
                    buttonsList.Add(button);
            }

            buttons = buttonsList.ToArray();
        }

        void Start()
        {
            foreach (var button in buttons)
                button.onClick.AddListener(() => OnClickButton(button));
        }

        void OnClickButton(Button button)
        {
            foreach (var b in buttons)
                b.interactable = button != b;
        }

#if UNITY_EDITOR
        [Button()]
        void FindButtons()
        {
            Undo.RecordObject(this, "FindButtons");
            Reset();
        }
#endif
    }
}