using System.Collections.Generic;
using UnityEngine;

namespace GamePackages.Localization
{
	public class TextVariables
	{  
		List<string>              keys       = new List<string>();
		Dictionary<string,string> keyToValue = new Dictionary<string, string>();

		public void SetValue(string key, string value)
		{  
			keyToValue[key] = value;
		}

		public string FillVariables(string text)
		{
			if (text == null)
				return null;
			
			bool   variableName = false;
			string currentKey   ="";
            
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
 
				if (variableName)
				{
					currentKey += c;
                    
					if (c == '}')
					{
						variableName = false;
						keys.Add(currentKey);
						currentKey = "";
						continue;
					}
				}
				else
				{
					if (c == '{')
					{
						currentKey   += c;
						variableName =  true;
						continue;
					}
				}
			}

			if (keys.Count > 0)
			{
				foreach (var key in keys)
				{
					if (keyToValue.ContainsKey(key))
					{
						text = text.Replace(key, keyToValue[key]); 
					}
				}

				keys.Clear();
			}

			return text;
		}
	}
}