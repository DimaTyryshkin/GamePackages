using System;
using UnityEngine;
using UnityEngine.Events;


namespace GamePackages.Localization
{
    public static class LocalizedKeyString
    {
        public static string NullKey => "system/null";
        static Base_LocalizedKeyString runTimeInst; 
 
        static bool isInit; 

        static void Init()
        {
            runTimeInst = new ResourcesLocalizedKeyString();
            runTimeInst.Init();
              
            isInit = true;
        }

        public static TextAsset[] LoadAllResourcedTextAssets(string resourceFolderPath)
        {
            return Resources.LoadAll<TextAsset>(resourceFolderPath); 
        }
 
        internal static void Reload()
        {
            Clear();
            Init();
        }

        public static void Clear()
        {
            isInit = false;
            runTimeInst = null; 
        }

        public static string[] GetTags(string key)
        {
            if (!isInit)
                Init();

            return runTimeInst.GetTags(key);
        }

        public static string GetString(string key)
        {
            if (!isInit)
                Init();

            return runTimeInst.GetString(key);
        }

        public static bool HaveValue(string key)
        {
            if (!isInit)
                Init();

            return runTimeInst.HaveValue(key);
        }
        
        public static Base_LocalizedKeyString GetData()
        {
            if (!isInit)
                Init();

            return runTimeInst;
        }
    }
}