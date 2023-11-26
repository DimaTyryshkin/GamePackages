using System;
using UnityEngine;

namespace GamePackages.JsonPlayerData
{
    [Serializable]
    public abstract class BaseAccountData
    {
        [Serializable]
        public class Metadata
        {
            public string appVersion;
            public string writeLocalTime;

            public void FillData()
            {
                appVersion = Application.version;
                writeLocalTime = DateTime.Now.ToString("G");
            }
        }

        /// <summary>
        /// Никогда нельзя изменять имя переменной, ее тип или удалять ее!
        /// </summary>
        public Metadata metadata = new Metadata();
		
        public virtual void Validate()
        {
            if (metadata == null)
                metadata = new Metadata();
        }
    }
}