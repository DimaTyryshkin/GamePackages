using GamePackages.Core.Validation;
using UnityEngine;

namespace GamePackages.Core
{
	class DebugMarkerCamera : MonoBehaviour
	{
		[SerializeField, IsntNull] Camera thisCamera;
		[SerializeField, IsntNull] Camera mainGameCamera;

		void Start()
		{
			if (!Application.isEditor)
			{
				thisCamera.gameObject.SetActive(false);
				gameObject.SetActive(false);
				return;
			}

			thisCamera.orthographic = mainGameCamera.orthographic;
		}

		void LateUpdate()
		{
			CameraExtension.Lerp(thisCamera, mainGameCamera, mainGameCamera, 1);
		}
	}
}