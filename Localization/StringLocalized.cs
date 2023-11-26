using System;
using GamePackages.Core.Validation;

namespace GamePackages.Localization
{
    [Serializable]
    public struct StringLocalized: IValidated
    {
        public static StringLocalized Null => new StringLocalized(LocalizedKeyString.NullKey);
        
        public enum StringCase
        {
            None = 0,
            Upper,
            Lower,
        }

        public string key;

        public string Text
        {
            get
            {
                if (HaveKey)
                {
                    return LocalizedKeyString.GetString(key);
                }
                else
                {
                    return "-NoKey-";
                }
            }
        } 
        
        bool HaveKey
        {
            get { return !string.IsNullOrEmpty(key); }
        }

        public bool HaveValue
        {
            get { return LocalizedKeyString.HaveValue(key); }
        }

        public bool IsNullKey => key == LocalizedKeyString.NullKey;

        public string[] TextTags
        {
            get { return LocalizedKeyString.GetTags(key); }
        }

        public StringLocalized(string key)
        {
            this.key = key;
        }

        public string GetTextCase(StringCase stringCase, string text)
        {
            if (stringCase == StringCase.Lower) return text.ToLower();
            if (stringCase == StringCase.Upper) return text.ToUpper();

            return text;
        }

        public override string ToString()
        {
            return Text;
        }

        public void Validate(ValidationContext context)
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(key))
            {
                context.AddProblem(nameof(StringLocalized), ValidationProblem.Type.Warning, "Не задан ключ локализации");
            }
            else
            {
                if (!LocalizedKeyString.HaveValue(key))
                    context.AddProblem(nameof(StringLocalized), ValidationProblem.Type.Warning, $"Нет локализации для ключа '{key}'");
            }
#endif
        }
    }
}