using System;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.UI; 

namespace GamePackages.Localization
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class LocalizedLabel : MonoBehaviour , IValidated
    {
        [SerializeField]
        StringLocalized str;
        
        [SerializeField]
        StringLocalized.StringCase stringCase;

        [Tooltip("{0} - Локализованная строка")]
        [SerializeField]
        string stringFormat;

        [NonSerialized]
        Text textComponent;

        void Start()
        {
            DrawLocalizedLabel(str.Text);
        }

        private void OnEnable()
        {
            LocalizationFacade.Reloaded += OnLocalizedStringReload;

            string text = null;
            text = str.Text;
            DrawLocalizedLabel(text);
        }

        private void OnDisable()
        {
            LocalizationFacade.Reloaded -= OnLocalizedStringReload;
        }

        private void OnLocalizedStringReload()
        {
            DrawLocalizedLabel(str.Text);
        }

        void DrawLocalizedLabel(string text)
        {
            string s = str.GetTextCase(stringCase, text); 

            if (!String.IsNullOrEmpty(stringFormat))
            {
                s = string.Format(stringFormat, s);
            }

            if (!textComponent)
                textComponent = GetComponent<Text>();
            
            textComponent.text = s;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Application.isPlaying == false)
                DrawLocalizedLabel(str.Text);
        }

//        void Update()
//        {
//            if (Application.isPlaying == false)
//            {
//                DrawLocalizedLabel();
//            }
//        }
#endif
        public void Validate(ValidationContext context)
        {
#if UNITY_EDITOR
            var text = GetComponent<Text>();
            if (!text)
                context.AddProblem(nameof(LocalizedLabel), ValidationProblem.Type.Error, "Text not found");
#endif
        }
    }
}