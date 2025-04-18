using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.JsonPlayerData
{
    [Serializable]
    public abstract class BaseAccountData
    {
        IStorage storage;

        /// <summary>
        /// Никогда нельзя изменять имя переменной, ее тип или удалять ее!
        /// </summary>
        public Metadata metadata = new Metadata();

        internal virtual void Validate()
        {
            if (metadata == null)
                metadata = new Metadata();
        }

        internal void SetStorage(IStorage storage)
        {
            Assert.IsNotNull(storage);
            this.storage = storage;
        }

        public void Save() => storage.Save();

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
    }
}