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

        [MenuItem("CONTEXT/Component/Set name")]
        static void SetName(MenuCommand command)
        {
            Component component = (Component)command.context;

            if (component)
            {
                string newName;
                GameObject go = component.gameObject;

                if (component is SpriteRenderer spriteRenderer)
                    newName = spriteRenderer.sprite.name;
                else
                    newName = component.GetType().Name;

                newName = Char.ToLower(newName[0]) + newName.Substring(1);
                go.RenameInEditor(newName);
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

