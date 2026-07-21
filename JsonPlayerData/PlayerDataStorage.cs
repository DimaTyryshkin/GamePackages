using System;
using Unity.Plastic.Newtonsoft.Json;
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

        JsonSerializerSettings serializerSettings;

        public PlayerDataStorage(IStringStorage jsonStorage)
        {
            Assert.IsNotNull(jsonStorage);
            this.jsonStorage = jsonStorage;

            serializerSettings = new JsonSerializerSettings()
            {
                Culture = System.Globalization.CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        string GetRawSaveData()
        {
            return jsonStorage.GetString("");
        }

        T LoadFromJson(string json)
        {
            try
            {
                data = JsonConvert.DeserializeObject<T>(json, serializerSettings);
                //var data = JsonUtility.FromJson<T>(json);
            }
            catch (JsonSerializationException e)
            {
                throw new Exception($"Не удалось прочитать файл с сохранением: {e.Path} {e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось прочитать файл с сохранением: " + e.Message);
            }

            Assert.IsNotNull(data);

            return data;
        }

        string ToJson(T data)
        {
            //return JsonUtility.ToJson(data, true);
            return JsonConvert.SerializeObject(data, serializerSettings);
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

        public void ReloadFromDisc()
        {
            data = null;
            jsonStorage.Reload();
        }

        public void Clear()
        {
            data = new T();
            Save();
        }
    }
}