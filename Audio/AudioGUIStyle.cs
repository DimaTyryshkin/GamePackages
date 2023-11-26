using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Audio
{
#if UNITY_EDITOR
	public static class AudioGUIStyle
	{
		public static int columnSizePlayBtn = 50;
		public static GUIStyle buttonLeftTextAlignmentStyle;

		public static void SelectObject(Object obj)
		{
			Selection.activeObject = obj;
			ProjectWindowUtil.ShowCreatedAsset(obj);
		}


		static AudioGUIStyle()
		{
			buttonLeftTextAlignmentStyle = new GUIStyle(GUI.skin.button);
			buttonLeftTextAlignmentStyle.alignment = TextAnchor.MiddleLeft;
		}
	}
#endif
}