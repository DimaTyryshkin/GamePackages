using System;
using System.Collections.Generic;
using System.IO;

namespace GamePackages.Localization
{
	public class CommentBlock : Block
	{
		public string commentText;

		public override void Write(TextWriter w )
		{
			w.WriteLine("//" + commentText);
			WriteEmptyLines(w);
		}

		public static CommentBlock Parse(List<string> lines, string fileName, ref int lineIndex)
		{
			CommentBlock c = new CommentBlock();

			var s = lines[lineIndex++];

			if (!s.StartsWith("//"))
				throw new Exception($"Can`t parse COMMENT in file '{fileName}' line '{lineIndex}'");

			c.commentText = s.Substring(2).Trim();

			c.ParseEmptyLines(lines, ref lineIndex);

			return c;
		}
	}
}