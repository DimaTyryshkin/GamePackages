using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
#endif 


namespace GamePackages.Core.Validation
{
    internal class UnityObjectsValidator : AbstractValidator
    {
        int imageCount;
        int spriteRendererCount;
        int particleSystemCount;

        public override int TotalValidationCount => imageCount + spriteRendererCount + particleSystemCount;

        public override void FindProblemsInField(object value, Type valueType, FieldInfo ownerFieldInfo, UnityEngine.Object rootObject)
        {

        }

        public override void FindProblemsInObject(object value, Type valueType, UnityEngine.Object rootObject)
        {
            if (value is UnityEngine.UI.Image image)
            {
                imageCount++;
                if (!image.sprite)
                    RecursiveValidator.ValidationContext.AddProblem(
                        nameof(UnityObjectsValidator),
                        ValidationProblem.Type.Warning,
                        "Image.sprite == null");
            }
            else if (value is UnityEngine.SpriteRenderer spriteRenderer)
            {
                spriteRendererCount++;
                if (!spriteRenderer.sprite)
                    RecursiveValidator.ValidationContext.AddProblem(
                        nameof(UnityObjectsValidator),
                        ValidationProblem.Type.Warning,
                        "SpriteRenderer.sprite == null");
            }
            else if (value is UnityEngine.ParticleSystem ps)
            {
                particleSystemCount++;
                var psr = ps.GetComponent<ParticleSystemRenderer>();
                if (psr.sortingLayerID == 0)
                    RecursiveValidator.ValidationContext.AddProblem(
                        nameof(UnityObjectsValidator),
                        ValidationProblem.Type.Warning,
                        $"ParticleSystem.sortingLayerID == 0 [{psr.sortingLayerName}]");
            }
        }

        public override string GetStats()
        {
            return $"UnityObjectsValidator Validator" + Environment.NewLine +
                   $"   Image полезных проверок = {imageCount}" + Environment.NewLine +
                   $"   SpriteRenderer полезных проверок = {spriteRendererCount}" + Environment.NewLine +
                   $"   ParticleSystem полезных проверок = {particleSystemCount}" + Environment.NewLine;
        }
    }
}