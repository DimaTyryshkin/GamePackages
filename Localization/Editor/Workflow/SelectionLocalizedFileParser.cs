using System;
using System.IO;
using UnityEngine;

namespace GamePackages.Localization.LocalizationWorkflow
{
	public class SelectionLocalizedFileParser:TextFileParser
	{
		SelectionLocalizedFile f;
 
		public SelectionLocalizedFile Result => f;
		
		public SelectionLocalizedFileParser(FileInfo file) : base(file)
		{
			f = new SelectionLocalizedFile();
			f.fullNameAfterSerialization = file.FullName;
		}

		protected override void ParseIteration(ref int lineIndex)
		{
			var s = lines[lineIndex];

			if (s.StartsWith(SelectionLocalizedFile.header))
			{
				f.targetAssetFilePath = s.Substring((SelectionLocalizedFile.header + " ").Length).Trim();
				lineIndex++;
				lineIndex++;//Там еще всегда пустая строка
				return;
			}

			var block = VoidTextBlock.Parse(lines, fileName, ref lineIndex);
			f.textBlocks.Add(block);
		}

		protected override void OnParseStart()
		{
			
		}

		protected override void OnParseFinish()
		{ 
		}
	}
}