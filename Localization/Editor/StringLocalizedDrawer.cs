using UnityEditor;
using UnityEngine; 

namespace GamePackages.Localization
{
    [CustomPropertyDrawer(typeof(StringLocalized))]
    public class StringLocalizedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Base_LocalizedKeyString data = LocalizedKeyString.GetData();
 
            EditorGUI.BeginProperty(position, label, property);
  
            position   = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

           
            int    indexOld = -1;
            string keyOld   = property.FindPropertyRelative("key").stringValue;

            string[] keys = data.Keys;
            for (int k = 0; k < keys.Length; k++)
            {
                if (keys[k] == keyOld)
                {
                    indexOld = k;
                    break;
                }
            }
            
            EditorGUI.BeginChangeCheck();
            int indexNew = EditorGUI.Popup(position, indexOld, keys);

            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("key").stringValue = keys[indexNew]; 
                Undo.RecordObject(property.serializedObject.targetObject, "ChangeKey");
 
                //string nameP = property.name;
                //Object obj   = property.serializedObject.targetObject;

                //Debug.Log(obj.name);
                //Debug.Log(property.propertyPath);
                //StringBrowserWindow.SrawForStrig(obj, property.propertyPath);// ------ На будущее
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}