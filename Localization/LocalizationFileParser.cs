using System.Collections.Generic;
using UnityEngine;
  
namespace GamePackages.Localization
{
	public class LocalizationFileParser : TextFileParser
	{
		LocalizedFile f;

		public LocalizedFile Result => f;
		
		public LocalizationFileParser(TextAsset textAsset) : base(textAsset)
		{
		}

		protected override void ParseIteration(ref int lineIndex)
		{
			string s = lines[lineIndex];

			if (s.StartsWith("//"))
			{
				var comment = CommentBlock.Parse(lines, fileName, ref lineIndex);
				f.blocks.Add(comment);

				return;
			}

			if (s.StartsWith("--")) //Начало групы
			{
				var keyBlock = KeyGroupBlock.Parse(lines, fileName, ref lineIndex);
				f.blocks.Add(keyBlock);

				return;
			}

			var recordBlock = LocalizedTextBlock.Parse(lines, fileName, ref lineIndex);

			f.blocks.Add(recordBlock);
		}

		protected override void OnParseStart()
		{
			f        = new LocalizedFile();
			f.blocks = new List<Block>();
			f.name   = fileName;

			f.assetFilePath = fileFullName;
		}

		protected override void OnParseFinish()
		{
			string lastKeyGroup = "";
			foreach (var block in f.blocks)
			{
				if (block is KeyGroupBlock k)
					lastKeyGroup = k.rawKey + "/";

				if (block is LocalizedTextBlock b)
				{
					b.fullKey = lastKeyGroup + b.rawKey;
				}
			}
		}
	}
}