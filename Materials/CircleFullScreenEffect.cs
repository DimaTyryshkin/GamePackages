using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace GamePackages.Materials
{
    public class CircleFullScreenEffect : MonoBehaviour
    {
        [SerializeField, IsntNull] Material myMaterial;
        public float radius = 0.5f;
        public Camera GameCamera;

        void LateUpdate()
        {
            Vector3 screenPoint = GameCamera.WorldToViewportPoint(transform.position, Camera.MonoOrStereoscopicEye.Mono);
            //Vector2 center = new Vector2(screenPoint.x / GameCamera.pixelWidth, screenPoint.y / GameCamera.pixelHeight);
            myMaterial.SetVector("_circleCenter", screenPoint);
            myMaterial.SetFloat("_circleRadius", radius);
        }

        [Button]
        public void ShowAll()
        {
            myMaterial.SetVector("_circleCenter", Vector4.zero);
            myMaterial.SetFloat("_circleRadius", 100);
        }

        public void HideAll()
        {
            myMaterial.SetVector("_circleCenter", Vector4.zero);
            myMaterial.SetFloat("_circleRadius", 0);
        }
    }
}