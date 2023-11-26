using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GamePackages.Localization
{
	public class LocalizedTextBlock : Block
	{
		public string   rawKey; //Ключ которые написан в самом файле
		public string   fullKey;
		public string[] tags;


		public List<LocalizedText> texts;

		public LocalizedText GetRecordByLanguage(string languageKey)
		{
			foreach (var t in texts)
			{
				if (t.languageKey == languageKey)
					return t;
			}

			return null;
		}

		public override void Write(TextWriter w)
		{
			w.Write(rawKey);

			if (tags != null)
			{
				w.Write(" ");
				w.Write(string.Join(" ", tags));
			}

			w.WriteLine();

			foreach (var r in texts)
				r.Write(w);

			WriteEmptyLines(w);
		}

		public static LocalizedTextBlock Parse(List<string> lines, string fileName, ref int lineIndex)
		{
			//Локального ключа еще небыло значит это он 
			var set = new LocalizedTextBlock();

			set.texts = new List<LocalizedText>();

			string s          = lines[lineIndex++];
			var    keyAndTags = s.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			string localKey   = keyAndTags[0];

			set.rawKey = localKey;

			if (keyAndTags.Length > 1)
				set.tags = keyAndTags.Skip(1).ToArray();
			else
				set.tags = null;

			while (lineIndex < lines.Count)
			{
				s = lines[lineIndex];

				if (IsEmptyLine(s)) //предположительно переход на другую строку
				{
					set.ParseEmptyLines(lines, ref lineIndex);

					if (set.texts.Count == 0)
						Debug.LogError("В файле " + fileName + ":line " + lineIndex + " для ключа " + set.rawKey + " нет ни одной строки");

					break;
				}
				else
				{
					string langKey =LocalizationSettings.ParseLanguage(s);

					if (string.IsNullOrEmpty(langKey))
					{
						//throw new Exception($"Can`t parse file '{fileName}' line '{lineIndex}' '{s}'");
						Debug.LogError($"Can`t parse file '{fileName}' line '{lineIndex}' '{s}'");
					}

					LocalizedText r = new LocalizedText();
					r.languageKey = langKey;
					r.text        = LocalizationSettings.GetTextFromLanguageLine(s);
					set.texts.Add(r);

					lineIndex++;
				}
			}

			return set;
		}
	}
}