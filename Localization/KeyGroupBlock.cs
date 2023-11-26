using System;
using System.Collections.Generic;
using System.IO;

namespace GamePackages.Localization
{
	public class KeyGroupBlock : Block
	{
		public string rawKey;

		public override void Write(TextWriter w )
		{
			w.WriteLine("--" + rawKey);
			WriteEmptyLines(w);
		}

		public static KeyGroupBlock Parse(List<string> lines, string fileName, ref int lineIndex)
		{
			KeyGroupBlock k = new KeyGroupBlock();

			var s = lines[lineIndex++];

			if (!s.StartsWith("--"))
				throw new Exception($"Can`t parse KeyGroupBlock in file '{fileName}' line '{lineIndex}'");

			k.rawKey = s.Substring(2).Trim();
			k.ParseEmptyLines(lines, ref lineIndex);

			return k;
		}
	}
}