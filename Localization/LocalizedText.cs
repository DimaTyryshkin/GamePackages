using System.IO;

namespace GamePackages.Localization
{
	public class LocalizedText
	{
		public string languageKey;
		public string text;

		public bool IsVoid => string.IsNullOrWhiteSpace(text);

		public void Write(TextWriter w)
		{
			w.Write(languageKey);
			w.Write(" ");
			w.Write(text);
			w.WriteLine();
		}
	}
}