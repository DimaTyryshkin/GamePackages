namespace GamePackages.Localization
{
	public struct LocalizedSet
	{
		public string   key;
		public string[] tags;

		public string originText;

		public string Text => LocalizationSettings.variables.FillVariables(originText);
	}
}