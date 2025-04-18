using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.JsonPlayerData
{
    public interface IPlayerPrefs
    {
        string GetString(string key, string defaultValue);
        void SetString(string key, string value);
        void Save();
    }

    public interface IStringStorage
    {
        string GetString(string defaultValue);
        void SetString(string value);
        void Save();
    }


    public class PlayerPrefsStringStorage : IStringStorage
    {
        readonly string key;
        readonly IPlayerPrefs playerPrefs;

        public PlayerPrefsStringStorage(string key, IPlayerPrefs playerPrefs)
        {
            Assert.IsNotNull(playerPrefs);

            this.key = key;
            this.playerPrefs = playerPrefs;
        }

        public string GetString(string defaultValue)
        {
            return playerPrefs.GetString(key, defaultValue);
        }

        public void SetString(string value)
        {
            playerPrefs.SetString(key, value);
        }

        public void Save()
        {
            playerPrefs.Save();
        }
    }

    public class FileStringStorage : IStringStorage
    {
        readonly FileInfo file;
        string rawJson;

        public FileStringStorage(FileInfo file)
        {
            Assert.IsNotNull(file);
            this.file = file;

            if (file.Exists)
            {
                using (var stream = file.OpenText())
                    rawJson = stream.ReadToEnd();
            }
        }

        public string GetString(string defaultValue)
        {
            if (rawJson == null)
                return defaultValue;

            return rawJson;
        }

        public void SetString(string value)
        {
            rawJson = value;
        }

        public void Save()
        {
            if (!file.Directory.Exists)
                file.Directory.Create();

            File.WriteAllText(file.FullName, rawJson);
        }
    }

    public class PlayerPrefsWrapper : IPlayerPrefs
    {
        public string GetString(string key, string defaultValue)
        {
            if (!PlayerPrefs.HasKey(key)) //удалять нельзя! UnityEngine.PlayerPrefs не умеет нормально работать если defaultValue==null 
                return defaultValue;

            return PlayerPrefs.GetString(key);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }
    }
}