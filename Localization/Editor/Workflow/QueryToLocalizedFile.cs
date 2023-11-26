namespace GamePackages.Localization.LocalizationWorkflow
{
	public class QueryToLocalizedFile
	{
		public string blockFullKey;
		public int    blockIndex;
		LocalizedFile file;

		public QueryToLocalizedFile(string blockFullKey, int blockIndex, LocalizedFile file)
		{
			this.blockFullKey = blockFullKey;
			this.blockIndex   = blockIndex;
			this.file         = file;
		}

		public LocalizedTextBlock Execute()
		{
			if (blockIndex >= file.blocks.Count)
				return null;
			
			var block = file.blocks[blockIndex];
			if (block is LocalizedTextBlock b)
			{
				if (b.fullKey == blockFullKey)
					return b;
			}

			return null;
		}
	}
}