using System;
using UnityEngine;
using GamePackages.Core.Validation;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using NaughtyAttributes;
using UnityEditor;
#endif

namespace GamePackages.Core
{
    public abstract class ObjectCollectionBase : ScriptableObject
    {
        #if UNITY_EDITOR
        public abstract void LoadFromCurrentDirectory(bool includeSubFolder);
        #endif
    }

    public class ObjectCollection<T> : ObjectCollectionBase where T : Object
    {
        [Serializable]
        public class ObjectCollectionException : Exception
        {  
            public ObjectCollectionException(string collectionName, string itemAlias) : base($"В коллекции '{collectionName}' отсутствует элемент '{itemAlias}'")
            {
            }
        }
        
        
        [IsntNull]
        public T[] collection;

        public int Count
        {
            get { return collection.Length; }
        }

        public void AssertElements()
        {
            AssertWrapper.IsAllNotNull(collection);
        }

        public T GetByIndex(int index)
        {
            return collection[index];
        }

        public T GetByName(string name)
        {
            foreach (var obj in collection)
            {
                if (obj.name == name)
                    return (T) obj;
            }

            throw new ObjectCollectionException(this.name,  name);
        }

        public int GetIndex(T obj)
        {
            for (int k = 0; k < collection.Length; k++)
            {
                if (obj == collection[k])
                    return k;
            }
 
            throw new ObjectCollectionException(name, obj.name);
        } 
        
        public int GetIndex(string name)
        {
            for (int k = 0; k < collection.Length; k++)
            {
                if (name == collection[k].name)
                    return k;
            }
 
            throw new ObjectCollectionException(name, name);
        } 

#if UNITY_EDITOR
        protected static void LoadInternal (MenuCommand command, bool includeSubFolder = false)
        {
            Debug.Log(command.context);
            Debug.Log(command.context.GetType().Name);
            
            var t = command.context as ObjectCollectionBase;
            t.LoadFromCurrentDirectory(includeSubFolder);
        }
 
        [Button]
        public void Load()
        {
            LoadFromCurrentDirectory(true);
        }

        public override void LoadFromCurrentDirectory(bool includeSubFolder)
        { 
            collection = AssetDatabaseWrapper.Load<T>(this, includeSubFolder).ToArray();
            EditorUtility.SetDirty(this);
        }
#endif
        
// #if UNITY_EDITOR
//         [Button()]
//         void Load()
//         {
//             LoadFromCurrentDirectory(true);
//         }
// #endif
    }
}