using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GamePackages.Tools
{
	public class RuntimeObjectsPalette : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] string[] targetNames;
        
		[NonSerialized]
		Dictionary<string, GameObject> nameToGo;

		
		void OnEnable()
		{
			
			SceneView.duringSceneGui += OnSceneGui;
		}

		void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneGui;
		}

		void OnSceneGui(SceneView obj)
		{  
			if (EditorApplication.isPaused)
			{
				if (nameToGo == null)
					FindAllTargets();

				GUI.color = Color.white;
				GUILayout.BeginArea(new Rect(10, 10, 200, 1000));
				{ 
					foreach (string targetName in targetNames)
					{
						GUI.enabled = nameToGo.ContainsKey(targetName);
						if (GUILayout.Button(targetName))
							Selection.activeGameObject = nameToGo[targetName];
					}

					GUI.enabled = true;
				}
				GUILayout.EndArea();
			}
			else
			{
				nameToGo = null;
			}
		}

		void FindAllTargets()
		{
			nameToGo = new Dictionary<string, GameObject>();
			foreach (string targetName in targetNames)
			{
				var go = GameObject.Find(targetName);
				if (go)
					nameToGo[targetName] = go;
			}
		}
#endif
	}
}