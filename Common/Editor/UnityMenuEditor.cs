using System;
using GamePackages.Core.Validation;
using UnityEditor; 
using UnityEngine;
 
namespace GamePackages.Core.Editor
{
    public static class UnityMenuEditor
    {
        [MenuItem("CONTEXT/ObjectCollectionBase/Load")]
        static void DoubleMass2(MenuCommand command)
        {
            ObjectCollectionBase collection = (ObjectCollectionBase)command.context;

            if (collection)
                collection.LoadFromCurrentDirectory(true);
        }   
        
        [MenuItem("CONTEXT/Component/CopyNameToGO")]
        static void DoubleMass(MenuCommand command)
        {
            Component component = (Component)command.context;
            
            if (component)
            {
                GameObject go = component.gameObject;
                string name = component.GetType().Name;
                string newName = Char.ToLower(name[0]) + name.Substring(1);
                Undo.RegisterCompleteObjectUndo(go, "rename");
                go.name = newName;
            }
        }   
         
        [MenuItem("GamePackages/Validate/All assets for build", false, 11)]
        public static void ValidateAll_MenuItem()
        {
            AssetsValidator.ValidateAll(); 
        }
         
        
        [MenuItem("GamePackages/Validate/Current scene", false, 12)]
        public static void ValidateCurrentScene_MenuItem() 
        {
            AssetsValidator.ValidateCurrentScene(); 
        }
        
       
    }
}

