using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.JsonPlayerData
{
    /* Пример инициализации
   public class GameFactory : MonoBehaviour
   {
       static PlayerDataStorage<AccountData> storage;
       public static string PlayerDataStorageFileName => Path.Combine(Application.persistentDataPath, "PlayerDataJsonString.json");

       public static PlayerDataStorage<AccountData> Storage
       {
           get
           {
               if (storage == null)
               {
                   IStringStorage stringStorage =
#if UNITY_EDITOR

                       new FileStringStorage(new FileInfo(PlayerDataStorageFileName));
#else
                       new PlayerPrefsStringStorage("PlayerDataJsonString", new PlayerPrefsWrapper());
#endif
                   storage = new PlayerDataStorage<AccountData>(stringStorage);
               }

               return storage;
           }
       }

       public static AccountData Data => Storage.GetDataSingleton();
   }
   */

    public interface IStorage
    {
        void Save();
        void Clear();
    }

    public class PlayerDataStorage<T> : IStorage
        where T : BaseAccountData, new()
    {
        T data;
        IStringStorage jsonStorage;



        public PlayerDataStorage(IStringStorage jsonStorage)
        {
            Assert.IsNotNull(jsonStorage);
            this.jsonStorage = jsonStorage;
        }

        public string GetRawSaveData()
        {
            return jsonStorage.GetString("");
        }

        public static T LoadFromJson(string json)
        {
            var data = JsonUtility.FromJson<T>(json);
            Assert.IsNotNull(data);

            return data;
        }

        public static string ToJson(T data)
        {
            return JsonUtility.ToJson(data, true);
        }

        T LoadFromJson_CatchExceptionDecorator(string json)
        {
            try
            {
                return LoadFromJson(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new T();
            }
        }

        public T GetDataSingleton()
        {
            if (data == null)
            {
                var json = jsonStorage.GetString("{}");
                //#if UNITY_EDITOR
                data = LoadFromJson(json);
                //#else
                //				data = LoadFromJson_CatchExceptionDecorator(json);
                //#endif
            }

            data.SetStorage(this);
            data.Validate();
            return data;
        }

        public void Save()
        {
            if (data == null)
                return;

            data.metadata = new BaseAccountData.Metadata();
            data.metadata.FillData();

            string json = ToJson(data);
            jsonStorage.SetString(json);
            jsonStorage.Save();
        }

        public void Clear()
        {
            data = new T();
            Save();
        }
    }
}