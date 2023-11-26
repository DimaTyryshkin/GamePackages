using UnityEngine;

namespace GamePackages.Localization.LocalizationWorkflow
{
	public class  LocalizedTextBlockMerge
	{
		LocalizedTextBlock from;
		LocalizedTextBlock to;

		public LocalizedTextBlockMerge(LocalizedTextBlock from, LocalizedTextBlock to)
		{
			this.from = from;
			this.to   = to;
		}

		public bool CanMakeCleanMerge()
		{
			if (from.GetRecordByLanguage("ru").text != to.GetRecordByLanguage("ru").text)
				return false;
 
			return true;
		}

		public void Merge()
		{
			foreach (var fromText in @from.texts)
			{
				if(fromText.languageKey=="ru")
					continue;
				
				if(fromText.IsVoid)
					continue;

				LocalizedText text = to.GetRecordByLanguage(fromText.languageKey);
				if (text == null)
				{
					text             = new LocalizedText();
					text.languageKey = fromText.languageKey;
					to.texts.Add(text);
				}
 
				text.text = fromText.text;
			}
		}
	}
}