using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Core.Validation
{
    public class NotNullFieldValidator : AbstractValidator
    {
        int isntNullValidationCount;
        int missingScriptValidationCount;

        public override int TotalValidationCount => isntNullValidationCount + missingScriptValidationCount;
        
        public NotNullFieldValidator()
        {
            
        }
        
        static bool IsSimilarToNull(object obj)
        {
            if (obj == null || obj.Equals(null))
            {
                return true;
            }
            else
            {
                if (obj is string s)
                    return string.IsNullOrWhiteSpace(s);

                return false;
            }
        }

        public override string GetStats()
        {
            return $"NotNull Validator" + Environment.NewLine +
                   $"NotNull полезных проверок = {isntNullValidationCount}" + Environment.NewLine +
                   $"   Missing script полезных проверок = {missingScriptValidationCount}" + Environment.NewLine;
        }

        public override void FindProblemsInField(object value, Type valueType, FieldInfo ownerFieldInfo, Object rootObject)
        {
#if UNITY_EDITOR
                var attribute = ownerFieldInfo.GetCustomAttribute<IsntNullAttribute>();

                if (attribute != null)
                {
                    isntNullValidationCount++;
                    if (IsSimilarToNull(value))
                    {
                        ValidationProblem.Type type = attribute.Warring ? ValidationProblem.Type.Warning : ValidationProblem.Type.Error;

                        RecursiveValidator.ValidationContext.AddProblem(nameof(IsntNullAttribute), type);
                    }
                }
#endif
        }

        public override void FindProblemsInObject(object value, Type valueType, Object rootObject)
        {
#if UNITY_EDITOR
            if (value is ScriptableObject so)
            {
                missingScriptValidationCount++;
                MonoScript s = MonoScript.FromScriptableObject(so);
                if (!s)
                    RecursiveValidator.ValidationContext.AddProblem("Missing script", ValidationProblem.Type.Error, overrideRoot: so);
            }

            if (value is MonoBehaviour mb)
            {
                missingScriptValidationCount++;
                MonoScript s = MonoScript.FromMonoBehaviour(mb);
                if (!s)
                    RecursiveValidator.ValidationContext.AddProblem("Missing script", ValidationProblem.Type.Error, overrideRoot: mb);
            }
#endif
        }
    }
}