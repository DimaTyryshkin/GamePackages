using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Core
{
    public static class GameObjectExtension
    {
        public static void SetEnable(this GameObject go, bool enable)
        {
            go.SetActive(enable);
        }

#if UNITY_EDITOR
        public static void RenameInEditor(this GameObject go, string newName)
        {
            Undo.RegisterCompleteObjectUndo(go, "rename");

            go.name = newName;

            string path = AssetDatabase.GetAssetPath(go);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, newName);
                EditorUtility.SetDirty(go);
            }
        }
#endif
        public static string FullName(this GameObject go)
        {
            string s = "";
            Transform t;
            t = go.transform;

            do
            {
                s = "/" + t.name + s;
                t = t.parent;
            } while (t);

            return s;
        }


    }
}