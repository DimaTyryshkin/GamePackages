using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Localization.LocalizationWorkflow
{
	public static class Workflow
	{
		static string resultFilesPath = "LocalizationWorkflow";
		
		public static void RewriteAllLocalizedFiles()
		{
			var allFiles = LoadAllLocalizedFilesInProject();

			foreach (var f in allFiles)
				f.RewriteToFile();
		}

		public static SelectionLocalizedFile[] LoadAllSelectionLocalizedFiles()
		{
			DirectoryInfo d     = new DirectoryInfo(resultFilesPath);
			var           files = d.GetFiles();

			var selectedFiles = files.Select(f =>
			{
				SelectionLocalizedFileParser p = new SelectionLocalizedFileParser(f);
				p.Parse();
				return p.Result;
			}).ToArray();

			return selectedFiles;
		}

		public static void MergeSelectedFilesAndOriginFiles()
		{
			var selectedFiles = LoadAllSelectionLocalizedFiles();
			var allLocalizedFiles = LoadAllLocalizedFilesInProject();
			
			foreach (var sFile in selectedFiles)
			{
				LocalizedFile lFile = allLocalizedFiles.FirstOrDefault(f => f.assetFilePath == sFile.targetAssetFilePath);
				if (lFile == null)
				{
					Debug.LogError($"Файл '{sFile.fullNameAfterSerialization}'  содержит строки для файла '{sFile.targetAssetFilePath}', но он не найден");
					continue;
				}

				foreach (var fromTextBlock in sFile.textBlocks)
				{
					QueryToLocalizedFile query       = new QueryToLocalizedFile(fromTextBlock.textBlock.fullKey, fromTextBlock.originIndexInFile, lFile);
					LocalizedTextBlock   targetBlock = query.Execute();
					if (targetBlock == null)
					{
						Debug.LogError($"Файл '{sFile.fullNameAfterSerialization}'  содержит строки для блока' {fromTextBlock.originIndexInFile}:{fromTextBlock.textBlock.fullKey}', но он не найден в файле '{lFile.assetFilePath}'");
						continue;
					}
					
					LocalizedTextBlockMerge merge = new LocalizedTextBlockMerge(fromTextBlock.textBlock, targetBlock);
					if (!merge.CanMakeCleanMerge())
					{
						Debug.LogError($"Файл '{sFile.fullNameAfterSerialization}'  содержит строки для блока' {fromTextBlock.originIndexInFile}:{fromTextBlock.textBlock.fullKey}', но он не может быть смержен в чистую'");
						continue;
					}
					 
					merge.Merge();
				}
			}

			foreach (var lFile in allLocalizedFiles)
			{
				lFile.RewriteToFile();
			}
			
			Debug.Log("Ok");
		}

		public static LocalizedFile[] LoadAllLocalizedFilesInProject()
		{
			List<TextAsset> allFiles = new List<TextAsset>();
			allFiles.AddRange(LocalizedKeyString.LoadAllResourcedTextAssets(LocalizationSettings.ResourceFilesPath));

			var allLocalizedFiles = allFiles.Select(f =>
			{
				LocalizationFileParser p = new LocalizationFileParser(f);
				p.Parse();
				return p.Result;
			}).ToArray();

			return allLocalizedFiles;
		}

		public static void ExtractVoidTextBlocksAndSaveToFiles(LocalizedFile[] files)
		{ 
			DirectoryInfo d               = new DirectoryInfo(resultFilesPath);

			if (d.Exists)
				d.Delete(true);

			d.Create();


			foreach (var textFile in files)
			{
				SelectionLocalizedFile f = ExtractVoidTextBlocks(textFile);

				if (f.textBlocks.Count > 0)
				{
					StringWriter w          = new StringWriter();
					FileInfo     resultFile = new FileInfo(Path.Combine(d.FullName, textFile.name + ".txt"));
					resultFile = GetOriginFileName(resultFile);

					f.Write(w);

					File.WriteAllText(resultFile.FullName, w.ToString());
				}
			}

			Debug.Log("Фалы с непереведеными строками сохранены в папке " + d.FullName);
		}

		public static FileInfo GetOriginFileName(FileInfo originFile)
		{
			FileInfo f = new FileInfo(originFile.FullName);

			string extension = originFile.Extension;
			string name      = Path.GetFileNameWithoutExtension(originFile.FullName);

			while (f.Exists)
			{
				name += "_";
				string newName = Path.GetDirectoryName(originFile.FullName) + Path.DirectorySeparatorChar + name + extension;
				f = new FileInfo(newName);
			}

			return f;
		}

		public static SelectionLocalizedFile ExtractVoidTextBlocks(LocalizedFile file)
		{
			SelectionLocalizedFile f = new SelectionLocalizedFile(); 
			f.targetAssetFilePath = file.assetFilePath;

			int blockIndex = 0;
			foreach (var block in file.blocks)
			{
				if (block is LocalizedTextBlock b)
				{
					var r = b.GetRecordByLanguage("en");
					if (r == null || r.IsVoid)
					{
						var voidBlock = new VoidTextBlock();
						voidBlock.originIndexInFile = blockIndex;
						voidBlock.textBlock         = b;
						f.textBlocks.Add(voidBlock);
					}
				}

				blockIndex++;
			}

			return f;
		}
	}
}