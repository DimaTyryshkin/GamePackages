using System;
using System.Collections.Generic;
using System.IO;

namespace GamePackages.JsonPlayerData
{
	class FakeFileSystem : IFileSystem
	{
		public List<string> paths = new List<string>();

		public void AddAvailablePath(params string[] paths)
		{
			this.paths.AddRange(paths);
		}
  
		public bool FileExists(string path)
		{
			return paths.Contains(path);
		}

		public bool DirExists(string path)
		{
			throw new System.NotImplementedException();
		}

		public string[] GetFiles(string path)
		{
			return paths.ToArray();
		}

		public void CreateDirectoryIfNotExist(string path)
		{
			
		}

		public void MoveFile(string path, string newFullFileName)
		{
			int index = paths.FindIndex(f => f == path);

			if (index >= 0)
			{
				paths[index] = newFullFileName;
			}
			else
			{
				throw new FileNotFoundException(path);
			}
		}

		public void CopyFile(string path, string newFullFileName)
		{
			if(paths.Contains(newFullFileName))
				throw  new InvalidOperationException();
			
			paths.Add(newFullFileName);
		}

		public void RemoveDir(string cacheDirPath)
		{
			throw new System.NotImplementedException();
		}
	}
}