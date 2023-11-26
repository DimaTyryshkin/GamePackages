using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace GamePackages.Localization.LocalizationWorkflow
{
	public class SelectionLocalizedFile
	{
		public static readonly string header = "origin_asset_file_path";

		public string              targetAssetFilePath;
		public string              fullNameAfterSerialization;
		public List<VoidTextBlock> textBlocks = new List<VoidTextBlock>();

		public void Write(StringWriter w)
		{
			w.Write(header + " ");
			w.WriteLine(targetAssetFilePath);
			w.WriteLine();

			foreach (var b in textBlocks)
			{
				b.Write(w);
			}
		}
	}
}