using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GamePackages.JsonPlayerData
{
	public interface IFileSystem
	{
		bool     FileExists(string path);
		bool     DirExists(string  path);
		string[] GetFiles(string   path);

		void CreateDirectoryIfNotExist(string path);
		void MoveFile(string olfFullFileName, string newFullFileName);
		void CopyFile(string olfFullFileName, string newFullFileName);
		void RemoveDir(string cacheDirPath);
	}

	public class RealFileSystem:IFileSystem
	{
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}
		
		public bool DirExists(string path)
		{
			return Directory.Exists(path);
		}

		public string[] GetFiles(string path)
		{
			DirectoryInfo d = new DirectoryInfo(path);
			return d.GetFiles()
				.Select(f=>f.FullName)
				.ToArray();
		}

		public void CreateDirectoryIfNotExist(string path)
		{
			Directory.CreateDirectory(path);
		}

		public void MoveFile(string olfFullFileName, string newFullFileName)
		{
			File.Move(olfFullFileName, newFullFileName);
		}

		public void CopyFile(string olfFullFileName, string newFullFileName)
		{
			File.Copy(olfFullFileName,newFullFileName, true );
		}

		public void RemoveDir(string dirPath)
		{
			Directory.Delete(dirPath, true);
		}
	}
}