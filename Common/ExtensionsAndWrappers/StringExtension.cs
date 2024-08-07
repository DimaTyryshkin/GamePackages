namespace GamePackages.Core
{
	public static class StringExtension
	{
		public static string TrimEnd(this string s, string end)
		{
			if (s.EndsWith(end))
			{
				int length    = s.Length;
				int endLength = end.Length;
				return s.Substring(0, length - endLength);
			}

			return s;
		}
	}
}