using System.Collections.Generic;
using GamePackages.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePackages.Tools
{
	public static class Lightmapping 
	{ 
		public static void EnableGeneratingLightmapDataForAllMeshes()
		{
			var activeScene = SceneManager.GetActiveScene();
			Debug.Log($"activeScene = {activeScene.name}");


			HashSet<string> meshes = new HashSet<string>(4096);
			foreach (var root in activeScene.GetRootGameObjects())
			{
				var allFilters = root.GetComponentsInChildren<MeshFilter>();
				foreach (var filter in allFilters)
				{
					var sharedMesh = filter.sharedMesh;
					if(!sharedMesh)
						continue;
					
					string path = AssetDatabase.GetAssetPath(sharedMesh);
					if (!string.IsNullOrWhiteSpace(path) && path.StartsWith("Assets"))
						meshes.Add(path);
				}
			}
			
			Debug.Log(meshes.ToStringMultiline());

			foreach (var path in meshes)
			{
				var modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
				modelImporter.generateSecondaryUV = true;
				modelImporter.SaveAndReimport();
				
				EditorUtility.SetDirty(modelImporter);
			}
		} 
	}
}