using System;
using System.Linq;
using GamePackages.Core;
using UnityEditor;
using UnityEngine;

namespace GamePackages.Core
{
	[CustomPropertyDrawer(typeof(AsEnumAttribute))]
	public class AsEnumPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				var      a      = attribute as AsEnumAttribute;
				string[] labels = AsEnumAttributeValues.GetAvailableLabels(a.ValuesSource);


				string curValue = property.stringValue;
				int    index    = labels.IndexOf(v => AsEnumAttributeValues.GetValueFromLabel(v)==curValue);

				EditorGUI.BeginChangeCheck();
				EditorGUI.BeginProperty(position, label, property);
				{
					if (index == -1)
					{
						position  = EditorGUI.PrefixLabel(position, label);
						GUI.color = Color.red;

						if (GUI.Button(position, curValue))
							index = 0;

						GUI.color = Color.white;

					}
					else
					{
						index = EditorGUI.Popup(position, property.displayName, index, labels);
					}
				}
				EditorGUI.EndProperty();

				if (EditorGUI.EndChangeCheck())
				{
					if (index >= 0)
					{
						string value = AsEnumAttributeValues.GetValueFromLabel(labels[index]);
						property.stringValue = value;
					}
				}
			}
			else
			{
				EditorGUILayout.HelpBox($"'{property.type}' is not supported by '{nameof(AsEnumAttribute)}'. Use 'string' instead", MessageType.Error);
			}
		} 
	}
}