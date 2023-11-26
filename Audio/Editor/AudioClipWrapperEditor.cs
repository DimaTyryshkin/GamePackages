using UnityEditor;
using UnityEngine;

namespace GamePackages.Audio
{
	[CustomEditor(typeof(AudioClipWrapper))]
	public class AudioClipWrapperEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			AudioClipWrapper setAudioClipWrapper = target as AudioClipWrapper;
			if (GUILayout.Button("Play"))
			{
				
				EditorAudioExtensions.PlayClip(setAudioClipWrapper, null);
			}
			
			GUILayout.Label($"Используется '{setAudioClipWrapper.useCount}' раз");
		}
	}
}