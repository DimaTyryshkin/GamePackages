using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Core.Validation
{
	/// <summary>
	/// Проходит по дереву объектов и находит проблемы при пимоши валидаторов. 
	/// </summary>
	public class RecursiveValidator
	{
		public class SearchScope : IDisposable
		{
			bool printFullInfo;

			//Progress
			int validatedUnityObjectsCount;
			int predictedTotalUnityObjectsCount;
			DateTime timeStart;

			public int ValidatedUnityObjectsCount => validatedUnityObjectsCount;

			public SearchScope(bool printFullInfo, int predictedTotalUnityObjectsCount)
			{
				ResetStats();

				if (predictedTotalUnityObjectsCount == 0)
					predictedTotalUnityObjectsCount = 1;
				
				this.printFullInfo = printFullInfo;
				this.predictedTotalUnityObjectsCount = predictedTotalUnityObjectsCount;
				timeStart = DateTime.Now;

				allScanedObject = new HashSet<object>();

#if UNITY_EDITOR
				if (printFullInfo)
					EditorUtility.DisplayProgressBar("Валидация", "", 0.0f);
#endif
			}

			public void AddProgress(int add)
			{
				validatedUnityObjectsCount += add;
#if UNITY_EDITOR
				if (printFullInfo)
					EditorUtility.DisplayProgressBar("Валидация", $"{validatedUnityObjectsCount}", validatedUnityObjectsCount / (float)predictedTotalUnityObjectsCount);
#endif
			}

			void IDisposable.Dispose()
			{
#if UNITY_EDITOR
				EditorUtility.ClearProgressBar();
#endif

				searchScope = null;

				totalTime = DateTime.Now - timeStart;
				PrintStats(printFullInfo);

				ResetStats();
			}

			public static SearchScope StartSearch(AbstractValidator[] validatorsForSearchProblems, bool printFullInfo, int predictedTotalObjectsCount)
			{
				AssertWrapper.IsAllNotNull(validatorsForSearchProblems);

				if (searchScope != null)
					throw new InvalidOperationException("Last search should be finished before start new search");

				searchScope = new SearchScope(printFullInfo, predictedTotalObjectsCount);
				validators = validatorsForSearchProblems.ToArray();
				validationContext = new ValidationContext();
				return searchScope;
			}
		}

		static SearchScope             searchScope;
		static ValidationContext       validationContext;
	 
		static HashSet<System.Object>  allScanedObject;
		
		public static ValidationContext ValidationContext => validationContext;

		static AbstractValidator[] validators;

		static int maxDepth = 0;
		static int validationWithInterfaceCount = 0;
		static int totalUnityObjects = 0; 
		static int totalObject = 0;
		static TimeSpan totalTime = new TimeSpan();
  
	 

		static void ResetStats()
		{
			validationContext  = null; 
			allScanedObject    = null;
			validators         = null;

			totalObject       = 0;
			totalUnityObjects       = 0;
			validationWithInterfaceCount       = 0;
			maxDepth    = 0; 
			totalTime         = new TimeSpan();
		}

		static void PrintStats(bool fullInfo)
		{
			var allScenes = validationContext.validationProblems
				.GroupBy(p => p.sceneName);


			foreach (var problemOnScene in allScenes)
			{
				Debug.Log($"--- <b>{problemOnScene.Key}</b> ---");
				foreach (var problem in problemOnScene.OrderBy(entry => entry.header))
				{
					if (problem.type == ValidationProblem.Type.Error)
						Debug.LogError(problem, problem.root);
					else
						Debug.LogWarning(problem, problem.root);
				}
			}

			if (fullInfo)
			{
				string msg = "Stats:" + Environment.NewLine;
				msg += "Elapsed time = " + Mathf.RoundToInt((float)totalTime.TotalSeconds) + " sec" + Environment.NewLine;
				msg += "Total Object = " + totalObject + Environment.NewLine;
				msg += "Total Unity Objects = " + totalUnityObjects + Environment.NewLine;
				msg += "MaxDepth = " + maxDepth + Environment.NewLine;

				int validationCount = 0;
				foreach (AbstractValidator validator in validators)
				{
					msg += Environment.NewLine + validator.GetStats();
					validationCount += validator.TotalValidationCount;
				}

				msg += Environment.NewLine;
				msg += $"{nameof(IValidated)} {Environment.NewLine}";
				msg += $"    {nameof(IValidated)} полезных проверок = {validationWithInterfaceCount} ";

				msg += Environment.NewLine;
				msg += Environment.NewLine;
				msg += $"Всего полезных проверок {validationCount + validationWithInterfaceCount}";
				msg += Environment.NewLine;

				Debug.Log(msg);
			}
		}

		static bool NeedInCheck(Type type)
		{ 
			// Тут надо бы кешировать.
			// А еще надо поменять принцып проверки. Проверять классы, которые сылаются на сборку с артиботами целевыми
			
			var assemblyName = type.Assembly.FullName; 
                                              
			//Debug.Log(type.Name + " " + assemblyName);
			
			if (assemblyName.StartsWith("UnityEngine."))
				return false; //Отличное место для проверки Image.sprite

			if (assemblyName.Contains("mscorlib"))
				return false;

			return true;
		}

		static void GetFieldsIncludeInherited(Type type, List<FieldInfo> result)
		{
			var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

			result.AddRange(fields);
			if (NeedInCheck(type.BaseType))
				GetFieldsIncludeInherited(type.BaseType, result);
		}

		public static void ValidateRecursively(UnityEngine.Object unityObject)
		{
			ValidateRecursively(unityObject, unityObject, null, 0);
		}

		static void ValidateRecursively(System.Object obj, Object root, FieldInfo field, int depth)
		{
			if (IsNull(obj))
				return;

			var type = obj.GetType();

			if (type.IsClass)
			{
				if (allScanedObject.Contains(obj))
					return;

				allScanedObject.Add(obj);
			}

			if (obj is GameObject go)
			{
				MonoBehaviour[] monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>();
				foreach (MonoBehaviour mb in monoBehaviours)
				{
					try
					{
						if (!mb)
							throw new ArgumentNullException("MonoBehaviour is null. It likely references a script that's been deleted.");

						ValidateRecursively(mb, mb, null, depth);
					}
					catch (ArgumentNullException e)
					{
						//Эта ветка перехватывает случаи Missing(Mono Script)
						ValidationContext.AddProblem("Missing script", ValidationProblem.Type.Error, e.Message + " object=" + go.FullName(), go);
					}
				}

				return;
			}

			if (!NeedInCheck(type))
				return;

			validationContext.currentRoot = root;
			validationContext.currentFieldInfo = field;

			
			if (obj is UnityEngine.Object unityObject)
			{
				if (unityObject is Component c)
					ValidateRecursively(c.gameObject, c.gameObject, null, depth);
				else if (unityObject is ScriptableObject so)
				{
					Object[] allAssetsAtPath = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(so)); // Вложенные ScriptableObject
					if (allAssetsAtPath.Length > 1)
					{
						foreach (Object asset in allAssetsAtPath)
							ValidateRecursively(asset, so, null, depth);
					}
				}

				validationContext.currentRoot = unityObject;
				validationContext.currentFieldInfo = field;
				root = unityObject;

				totalUnityObjects++;
				searchScope.AddProgress(1);
			}

			totalObject++;

			if (depth > maxDepth)
				maxDepth = depth;

			foreach (var v in validators)
				v.FindProblemsInObject(obj, obj?.GetType(), root);

			if (obj is IValidated validated)
			{
				validationWithInterfaceCount++;
				try
				{
					validated.Validate(validationContext);
				}
				catch (Exception e)
				{
					validationContext.AddProblem("Exception while validate", ValidationProblem.Type.Error, e.ToString(), root);
				}
			}

			List<FieldInfo> fields = new List<FieldInfo>();
			GetFieldsIncludeInherited(obj.GetType(), fields);

			foreach (FieldInfo f in fields)
			{
				validationContext.currentFieldInfo = f;

#if UNITY_EDITOR
				//Поля не сереализуемые в редакторе скипаем если игра не запущена. Они будут заполнены только в рантайме.
				if (!f.IsPublic && EditorApplication.isPlaying == false)
				{
					SerializeField sf = f.GetCustomAttribute<SerializeField>();
					if (sf == null)
						continue;
				}
#endif

				object fieldObject = f.GetValue(obj);

				foreach (var v in validators)
					v.FindProblemsInField(fieldObject, fieldObject?.GetType(), f, root);

				ValidateRecursively(fieldObject, root, f, depth + 1);
				validationContext.currentRoot = root;
				validationContext.currentFieldInfo = f;


				if (fieldObject is ICollection collection) //Нельзя ходить по IEnumareble, так как будем залазить в символы строки=(
				{
					foreach (var item in collection)
					{
						foreach (var v in validators)
							v.FindProblemsInField(item, item?.GetType(), f, root);

						ValidateRecursively(item, root, f, depth + 1);
						validationContext.currentRoot = root;
						validationContext.currentFieldInfo = f;
					}
				}
			}
		}

		static bool IsNull(object obj)
		{ 
			if (obj == null || obj.Equals(null))
			{
				return true;
			}
			else
			{  
				return false;
			}
		} 
	}
}