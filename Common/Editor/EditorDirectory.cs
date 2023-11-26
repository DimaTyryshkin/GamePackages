using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using UnityEditor;

namespace GamePackages.Common
{
    public class EditorDirectory<T>
    {
        string dirName;
        public Dictionary<string, EditorDirectory<T>> directories = new Dictionary<string, EditorDirectory<T>>();
        public List<T> files = new List<T>();
        
        public bool IsFoldOutOnBrouser
        {
            get => EditorPrefs.GetBool("GamePackages.Common.EditorDirectory" + dirName);
            set => EditorPrefs.SetBool("GamePackages.Common.EditorDirectory" + dirName, value);
        }

        public EditorDirectory(string dirName)
        {
            this.dirName = dirName;
        }

        public EditorDirectory<T> GetOrCreate(string dirName)
        {
            return directories.GetOrCreate(dirName, ()=> new EditorDirectory<T>(dirName));
        }

        public void AddFile(T file)
        {
            files.Add(file);
        }

        public bool Validate()
        {
            if(files.Any(x=> x==null))
                return false;
            
            if(directories.Any(x=> !x.Value.Validate()))
                return false;

            return true;
        }
    }
}
