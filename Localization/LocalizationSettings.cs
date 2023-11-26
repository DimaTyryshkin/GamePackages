using System;
using System.Globalization;
using GamePackages.JsonPlayerData;
using UnityEngine;
   
namespace GamePackages.Localization
{
	public static class LocalizationSettings
	{
		public static readonly string ResourceFilesPath = "Localization/Files/";

		public static TextVariables variables = new TextVariables();

		static readonly string bsdKey = typeof(LocalizationSettings).Name + ".language";

		static CultureInfo cultureInfoCurent;

		public static CultureInfo CultureInfo
		{
			get { return cultureInfoCurent; }
		}

		static public int LanguageChangeIteration { get; private set; }

		public static readonly CultureInfo En                 = new CultureInfo("en");
		public static readonly CultureInfo Ru                 = new CultureInfo("ru");
		public static readonly CultureInfo Cn                 = new CultureInfo("zh-Hans");
		static readonly        CultureInfo cultureInfoDefault = En;

		static LocalizationSettings()
		{
			LanguageChangeIteration = 1;

			cultureInfoCurent = cultureInfoDefault;

			if (Application.systemLanguage == SystemLanguage.Russian)
				cultureInfoCurent = Ru;

			if (Application.systemLanguage == SystemLanguage.Chinese ||
			    Application.systemLanguage == SystemLanguage.ChineseSimplified ||
			    Application.systemLanguage == SystemLanguage.ChineseTraditional)
				cultureInfoCurent = Cn;

			try
			{
				string langugeName = PlayerPrefs.GetString(bsdKey, cultureInfoCurent.TwoLetterISOLanguageName);

				// с китайским какая-то дичь иногда бывает
				if (langugeName == Cn.TwoLetterISOLanguageName)
				{
					cultureInfoCurent = Cn;
					return;
				}

				cultureInfoCurent = new CultureInfo(langugeName);
			}
			catch (Exception exc)
			{
				Debug.LogError(exc.Message);
			}
		}
		 
		public static void SetLanguage(CultureInfo cultureInfo)
		{
			LanguageChangeIteration++;
			cultureInfoCurent = cultureInfo;
			PlayerPrefs.SetString(bsdKey, cultureInfo.TwoLetterISOLanguageName);
		}

	 
		public static readonly string[] LanguageKeys = new[]
		{
			"ru",
			"en",
			"zh"
		};

		public static string DecodeTagsToText(string str)
		{
			str = str.Replace("<br>", Environment.NewLine);
			str = str.Replace("<tab>", "    ");
			return str;
		}

		public static string GetTextFromLanguageLine(string languageLine)
		{
			if (languageLine.Length > 3) //Начиная с 3 индекса должна идти сама строка
			{
				return languageLine.Substring(3).Trim();
			}
			else
			{
				return "";
			}
		}

		public static string ParseLanguage(string line)
		{
			string langKey = line.Split(' ')[0];

			foreach (var availableKey in LocalizationSettings.LanguageKeys)
			{
				if (availableKey == langKey)
					return langKey;
			}

//			string s = langKey + " ";
//			string msg = "";
//			foreach (char c in s)
//			{
//				msg += (int) c + " ";
//			}
//			Debug.Log(msg);
//			Debug.Log($"{langKey}");
			return null;
		}
	}
}