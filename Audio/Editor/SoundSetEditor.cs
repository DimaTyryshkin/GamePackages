using UnityEditor;
using UnityEngine;

namespace GamePackages.Audio
{
	[CustomEditor(typeof(SoundsSet))]
	public class SoundSetEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Play"))
			{
				var set = target as SoundsSet;
				EditorAudioExtensions.PlayClip(set.NextClip(), set.AudioMixerGroup);
			}
		}
	}
}