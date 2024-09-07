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

		public DebugMarker Duration(float duration)
		{
			if (Application.isPlaying)
				Destroy(gameObject, duration);
			else
				EditorCoroutine.CallWithDelay(() =>
				{
					if (gameObject)
						DestroyImmediate(gameObject);
				}, duration);

			return this;
		}
	}
}