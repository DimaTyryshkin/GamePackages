using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace GamePackages.Localization
{
	public abstract class TextFileParser
	{
		/// <summary>
		/// Доустимые символы
		/// </summary>
		static string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
		                         "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ" +
		                         "abcdefghijklmnopqrstuvwxyz" +
		                         "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
		                         "0123456789" +
		                         " .,;:!?-'*()[]$@%&" +
		                         "\"" +       //кавычка
		                         "_#/<>={}" + //Символы для html тегов и прочей программисткой  штуки 
		                         "“”–—«»’";    //Особые символы
		

		protected List<string> lines;
		protected string       fileName;
		protected string       fileFullName;

		TextAsset textAsset;


		public TextFileParser(TextAsset textAsset)
		{
			Assert.IsNotNull(textAsset);
			this.textAsset = textAsset; 

			fileName = textAsset.name;
#if UNITY_EDITOR
			fileFullName = AssetDatabase.GetAssetPath(textAsset);
#endif
			StringReader r = new StringReader(textAsset.text);

			ReadLines(r);
			textAsset = null;
		}

		public TextFileParser(FileInfo file)
		{
			fileName = Path.GetFileNameWithoutExtension(file.FullName);
			StreamReader r = new StreamReader(file.FullName);

			ReadLines(r);
		}

		void ReadLines(TextReader r)
		{
			lines = new List<string>();
			string line = "";

			int n = 0;
			while (true)
			{
				n++;
				line = r.ReadLine();
				if (line == null)
					break;

				ValidateLine(line, n);
				line = CorrectLine(line);
				lines.Add(line);
			}
		}

		void ValidateLine(string line, int n)
		{
			foreach (char c in line)
			{
				bool isOk = false;
				foreach (char alphabetChar in alphabet)
				{
					if (alphabetChar == c)
					{
						isOk = true;
						break;
					}
				}

				if (!isOk)
					Debug.LogError($"[TextFileParser] file='{fileName}' содержит в строке '{n}' недопустимый символ '{c}'({(int)c})" + Environment.NewLine + line, textAsset);
			}
		}

		string CorrectLine(string line)
		{
			line = line.Replace("…", "...");
			line = line.Replace(((char)160).ToString(), " ");//Неразрывный пробел заменяем обычным пробелом
			return line;
		}

		public void Parse()
		{
			OnParseStart();

			int lineIndex = 0;
			while (lineIndex < lines.Count)
			{
				ParseIteration(ref lineIndex);
			}

			OnParseFinish();
		}

		protected abstract void ParseIteration(ref int lineIndex);
		protected abstract void OnParseStart();
		protected abstract void OnParseFinish();
	}
}