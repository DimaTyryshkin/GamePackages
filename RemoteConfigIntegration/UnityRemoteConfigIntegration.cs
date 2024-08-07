using System;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_CLOUD_SERVICES_RemoteConfig
using Unity.Services.RemoteConfig;
#endif

namespace GamePackages.RemoteConfigIntegration
{
#if ENABLE_CLOUD_SERVICES_RemoteConfig // Add manually in build settings
    public class UnityRemoteConfigIntegration<T> where T : class, new()
    {  
        //string log = "";

        public Version MinWindowsVersion { get; private set; }
        public Version MaxWindowsVersion { get; private set; }
        public string UpdateWindowsUrl { get; private set; } = "https://vkplay.ru/play/game/top_view_race";

        public string NewVersionInfo { get; private set; }
        public T Config { get; private set; }


        UnityAction complete;
        
        public void FetchConfigsAsync(UnityAction complete)
        {
            this.complete = complete;
            MinWindowsVersion = new Version(0, 0, 0);
            MaxWindowsVersion = new Version(Application.version);
            //GooglePlayMarketUrl = "market://details?id=com.DefaultCompany.Geo";
            //GooglePlayUrl = "https://play.google.com/store/apps/details?id=com.DefaultCompany.Geo";
            NewVersionInfo = "Доступна новая версия";
            Config = new T();
            
           
            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes()); 
        }
        
        public void DisplayAllKeys()
        {
            
            var keys = RemoteConfigService.Instance.appConfig.GetKeys();
            string allKeys = $"Current Keys (count={keys.Length}):" + Environment.NewLine;
            foreach (string key in keys)
            {
                string value = RemoteConfigService.Instance.appConfig.GetString(key);
                allKeys += $"    {key}={value}" + Environment.NewLine;
            }

            Debug.Log(allKeys);
        }

      

        void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            if (configResponse.status == ConfigRequestStatus.Success)
            {
                DisplayAllKeys();
                switch (configResponse.requestOrigin)
                {
                    case ConfigOrigin.Cached:
                    case ConfigOrigin.Remote:
                        var config = RemoteConfigService.Instance.appConfig;
                        MinWindowsVersion = new Version(config.GetString("min_windows_version"));
                        MaxWindowsVersion = new Version(config.GetString("max_windows_version"));
                        UpdateWindowsUrl = config.GetString("update_windows_url");
                        NewVersionInfo = config.GetString("new_version_info");

                        Config = JsonUtility.FromJson<T>(config.GetJson("config"));
                        if (Config == null)
                            Config = new T();
                        
                        break;
                } 
            } 
            
            complete?.Invoke();
        } 
        
        public struct userAttributes
        {
        }

        public struct appAttributes
        {
        }
    }
#endif
}