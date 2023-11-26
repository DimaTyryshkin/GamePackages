using GamePackages.Core;
using UnityEditor;
using UnityEngine;

namespace GamePackages.ForestGenerator
{
	 
	//[CustomPropertyDrawer(typeof(ForestEpoch))]
	public class ForestEpochPropertyDrawer : PropertyDrawer
	{ 
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * 2;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent2 = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			{
				var p0 = position.SplitGuiRectOnLines(2, 0);
				EditorGUI.PropertyField(p0, property.FindPropertyRelative("age"), new GUIContent("Age"));
				EditorGUI.PropertyField(p0, property.FindPropertyRelative("scale"),new GUIContent("Scale"));
			}
			EditorGUI.indentLevel = indent2; 
			EditorGUI.EndProperty();
		 
			 
		}
	}
}