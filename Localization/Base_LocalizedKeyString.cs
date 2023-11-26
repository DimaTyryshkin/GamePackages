using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using UnityEngine;

namespace GamePackages.Localization
{
	public abstract class Base_LocalizedKeyString
	{ 
		Dictionary<string, LocalizedSet> allSets = new Dictionary<string, LocalizedSet>(); 
  
		string[] keys;
		public string[] Keys => keys;


		public void Init()
		{
			allSets.Add(LocalizedKeyString.NullKey, new LocalizedSet()
			{
				key        = LocalizedKeyString.NullKey,
				tags       = null,
				originText = null,
			});
            
			LoadStrings(); 

			keys = allSets.Keys.ToArray();
		}

		TextAsset[] LoadAllResourcedTextAssets(string resourceFolderPath)
		{
			return Resources.LoadAll<TextAsset>(resourceFolderPath);
		}

		protected abstract TextAsset[] LoadTextsFromBundles();

		public void Reload()
		{
			Clear();
			Init();  
		}

		public virtual void Clear()
		{
			keys    = new string[0];
			allSets = new Dictionary<string, LocalizedSet>();
		}

		void LoadStrings()
		{
			List<TextAsset> allText = new List<TextAsset>();

			var resourcedTextAssets = LoadAllResourcedTextAssets(LocalizationSettings.ResourceFilesPath);
			allText.AddRange(resourcedTextAssets);

			//TextAsset[] texts = LocalizationSettings.TextsBundle.LoadAllAssets<TextAsset>();
			TextAsset[] bundledTextsAssets = LoadTextsFromBundles();
			allText.AddRange(bundledTextsAssets);
            
			foreach (var text in allText)
			{
				LocalizationFileParser p = new LocalizationFileParser(text);
				p.Parse();
				var localizedSet = p.Result.ExtractSets(LocalizationSettings.CultureInfo.TwoLetterISOLanguageName);

				foreach (var set in localizedSet)
				{
					if (allSets.ContainsKey(set.key))
						Debug.LogErrorFormat("Ключ {0} уже занят строкой<{1}>. Однако происходить запись новой строки <{2}> из файла {3} ", set.key, allSets[set.key].originText, set.originText, text.name);
					else
						allSets.Add(set.key, set);
				}
			}
		}

		public string[] GetTags(string key)
		{
			if (key == null) 
				return null;
  
			LocalizedSet result;

			if (allSets.TryGetValue(key, out result))
			{
				return result.tags;
			}
			else
			{
				return null;
			}
		}

		public string GetString(string key)
		{ 
			if (key == null) 
				return null;
            
			LocalizedSet result;

			if (allSets.TryGetValue(key, out result))
			{
				return  result.Text;
			}
            
			Debug.LogWarning("Ключ " + key + " не найден");
			// Debug.Log(GetAllStringsList());
			return key;
		}

		public bool HaveValue(string key)
		{
			if (key == null) 
				return false;
 
			return allSets.ContainsKey(key);
		}

		string GetAllStringsList()
		{
			StringBuilder result = new StringBuilder();
			result.AppendLine("Все ключи");
			foreach (KeyValuePair<string, LocalizedSet> set in allSets.OrderBy(s=>s.Key))
			{
				result.AppendLine(set.Key);
			}

			return result.ToString();
		}
	}
}