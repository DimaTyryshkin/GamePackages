using System.Linq;
using GamePackages.Common;
using UnityEditor;
using UnityEngine;

namespace GamePackages.Core
{
    public class DirectoryTreeBuilder<T> where T : Object
    {
        public EditorDirectory<T> rootDirectory = new EditorDirectory<T>("Sounds");
        //path: Assets/...
        public void LoadAllFiles(string path)
        {
            var files = EditorExtension.LoadAllAssetsOfType<T>(path, includeSubFolder: true).ToList();

            foreach (var file in files)
            {
                AddFileToFolder(file, GetFileName(file, path));
            }
        }
        void AddFileToFolder(T file, string filePath)
        {
            EditorDirectory<T> directory      = rootDirectory;
            var                directoryNames = filePath.Split('/');
            for (int i = 0; i < directoryNames.Length - 1; i++)
            {
                directory = directory.GetOrCreate(directoryNames[i]);
            }

            directory.AddFile(file);
        }

        static string GetFileName(T file, string rootPath)
        {
            return AssetDatabase.GetAssetPath(file).Replace($"{rootPath}/", "");
        }
    }
}