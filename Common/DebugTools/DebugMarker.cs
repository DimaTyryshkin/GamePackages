using GamePackages.Core.Validation;
using UnityEngine;

namespace GamePackages.Core
{
    public class DebugMarker : MonoBehaviour
    {
        [SerializeField, IsntNull] TextMesh text;

        public DebugMarker Text(string text)
        {
            this.text.text = text;
            return this;
        }

        public DebugMarker Text<T>(T toText)
        {
            return Text(toText.ToString());
        }

        public DebugMarker Duration(float duration)
        {
            if (Application.isPlaying)
                Destroy(gameObject, duration);
            else
            {
#if UNITY_EDITOR
                EditorCoroutine.CallWithDelay(() =>
                {
                    if (gameObject)
                        DestroyImmediate(gameObject);
                }, duration);
#endif
            }

            return this;
        }
    }
}