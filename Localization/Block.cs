using System;
using System.Collections.Generic;
using System.IO;

namespace GamePackages.Localization
{ 
	public abstract class Block
	{
		public          int  emptyLinesCount;
		public abstract void Write(TextWriter w);

		protected void WriteEmptyLines(TextWriter w)
		{
			if (emptyLinesCount == 0)
				emptyLinesCount = 1;
			
			for (int i = 0; i < emptyLinesCount; i++)
				w.WriteLine();
		}

		public void ParseEmptyLines(List<string> lines, ref int lineIndex)
		{
			bool read = true;
			while (read && lineIndex < lines.Count)
			{
				var s = lines[lineIndex];

				if (IsEmptyLine(s))
				{
					emptyLinesCount++;
					lineIndex++;
				}
				else
				{
					return;
				}
			}
		}

		protected static bool IsEmptyLine(string s)
		{
			var trimS = s.Trim();
			return string.IsNullOrWhiteSpace(trimS) || trimS == Environment.NewLine;
		}


	}
}