using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePackages.Core.Validation
{
	public static class AssetsValidator
	{
		public static void ValidateCurrentScene()
		{
			using (var scope = RecursiveValidator.SearchScope.StartSearch(GetValidators(), true, 0))
			{  
				ValidateCurrentSceneInternal();
			}
		}
		
		public static void ValidateAll()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				Scene scene = SceneManager.GetSceneAt(0); 
				EditorCoroutine.Start(ValidateAllAndOpenSceneCoroutine(scene));
			}
		}

		static IEnumerator ValidateAllAndOpenSceneCoroutine(Scene scene)
		{
			string path = scene.path;
			yield return ValidateAllCoroutine();
			EditorSceneManager.OpenScene(path);
		}

		static IEnumerator ValidateAllCoroutine()
		{
			// Будем показывать прогресс валидации, основываясь на колличестве объектов в предыдущей запущеной валидации
			string bsdKey = nameof(AssetsValidator) + "Last-ValidatedUnityObjectsCount";
			int predictedTotalUnityObjectsCount = EditorPrefs.GetInt(bsdKey,0); 
			
			using (var scope = RecursiveValidator.SearchScope.StartSearch(GetValidators(), true, predictedTotalUnityObjectsCount))
			{
				yield return ValidateAllScenes();
				ValidateResources();
				
				EditorPrefs.SetInt(bsdKey, scope.ValidatedUnityObjectsCount);
			}
		}

		static IEnumerator ValidateAllScenes()
		{  
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes; 
 
			foreach (var scene in scenes)
			{ 
				if(scene.enabled)
				{
					EditorSceneManager.OpenScene(scene.path);
					yield return null;
					ValidateCurrentSceneInternal();
				} 
			}
		}
 
		static void ValidateCurrentSceneInternal()
		{
			var scene = SceneManager.GetSceneAt(0); 
			GameObject[] rootSceneGameObjects = scene.GetRootGameObjects();

			foreach (GameObject rootGameObjectInScene in rootSceneGameObjects)
			{
				RecursiveValidator.ValidateRecursively(rootGameObjectInScene); 
			}
		}

		static void ValidateResources()
		{
			Object[] allResources = Resources.LoadAll("");

			foreach (Object obj in allResources)
				RecursiveValidator.ValidateRecursively(obj);
		}

		static AbstractValidator[] GetValidators() 
		{
			return new AbstractValidator[]
			{
				new NotNullFieldValidator(),
				new AsEnumFieldValidator(), 
			};
		}
	}
}