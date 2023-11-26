using System;
using System.Collections.Generic;
using System.IO;

namespace GamePackages.Localization.LocalizationWorkflow
{
	public class VoidTextBlock : Block
	{
		public int                originIndexInFile; 
		public LocalizedTextBlock textBlock;
		
		public override void Write(TextWriter w)
		{
			w.Write(originIndexInFile);
			w.Write(" ");
			w.WriteLine(textBlock.fullKey);
			textBlock.Write(w);
		}

		public static VoidTextBlock Parse(List<string> lines, string fileName, ref int lineIndex)
		{
			VoidTextBlock b = new VoidTextBlock();
			
			var s = lines[lineIndex++];
			int originIndex;
			if (!int.TryParse(s.Split(' ')[0], out originIndex))
			{
				throw  new Exception($"Cant parse file '{fileName}' at line {lineIndex}");
			}

			string fullKey = s.Substring(originIndex.ToString().Length + 1);

			b.originIndexInFile = originIndex;
			b.textBlock         = LocalizedTextBlock.Parse(lines, fileName, ref lineIndex);
			b.textBlock.fullKey = fullKey;

			return b;
		}
	}
}