using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePackages.Core.Validation
{

    /// <summary>
    /// Not null violation represents data for objects that do not have their required (NotNull) fields  
    /// assigned.
    /// </summary>
    public class ValidationProblem
    {
        public enum Type
        {
            Error = 0,
            Warning = 1,
        }

        public Object root;
        public FieldInfo fieldInfo;
        public string header;
        public string msg;
        public string sceneName { get; private set; }
        public Type type;

        string logMsg;



        string FullName
        {
            get
            {
                if (root is MonoBehaviour mb)
                    return $"{mb.gameObject.FullName()}[{root.GetType().Name}]";

                if (root is GameObject go)
                    return go.FullName();

                return root.name;
            }
        }

        public void CacheToString()
        {
            logMsg = ConvertToString();
        }

        string ConvertToString()
        {
            string log = $"<b>{header} {msg}</b>" + Environment.NewLine;
            log += $" FullName={FullName}" + Environment.NewLine;

            if (fieldInfo != null)
                log += $" Member={fieldInfo.Name}" + Environment.NewLine;

#if UNITY_EDITOR
            var pathToRoot = AssetDatabase.GetAssetPath(root);

            if (!string.IsNullOrWhiteSpace(pathToRoot))
            {
                log += $" Path={pathToRoot}" + Environment.NewLine;
                sceneName = "Assets";
            }
            else
            {
                if (root is Component c)
                {
                    string goSceneName = c.gameObject.scene.name;
                    if (!string.IsNullOrWhiteSpace(goSceneName))
                        sceneName = goSceneName;
                }
            }

            if (!string.IsNullOrWhiteSpace(sceneName))
                log += $" Scene={sceneName}" + Environment.NewLine;
#endif
            log += Environment.NewLine;
            return log;
        }

        public override string ToString()
        {
            return logMsg;
        }
    }
}
