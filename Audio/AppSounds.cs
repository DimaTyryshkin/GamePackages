using System.Collections.Generic;
using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GamePackages.Audio
{
    public class SoundGroup
    {
        readonly SoundGroupSettings settings;
        SoundSetPlayer oldPlayer;
        
        public float timeNextPlay;

        public SoundGroup(SoundGroupSettings settings)
        { 
            this.settings = settings;
        }

        public void SetNewPlayer(SoundSetPlayer player)
        {
            if (oldPlayer)
                oldPlayer.Stop();

            oldPlayer = player;
            timeNextPlay = Time.realtimeSinceStartup + settings.minDelayToNextPlay;
        }

        public void OnPlayerStop(SoundSetPlayer soundSetPlayer)
        {
            Assert.AreEqual(soundSetPlayer, oldPlayer);
            oldPlayer = null;
        }
    }

    public class AppSounds : MonoBehaviour
    { 
        public static float Volume
        {
            get { return instScene.data.Volume; }
            set
            {
                instScene.data.Volume = value;
                VolumeChanged?.Invoke();
            }
        }
        
        public static float MusicVolume
        {
            get => instScene.data.MusicVolume;
            set
            {
                instScene.data.MusicVolume = value;
                VolumeChanged?.Invoke();
            }
        }


        public static float SoundsVolume
        {
            get => instScene.data.SoundVolume;
            set
            {
                instScene.data.SoundVolume = value;
                VolumeChanged?.Invoke();
            }
        }

        public static event UnityAction VolumeChanged;

        
        [SerializeField, IsntNull] AppSoundsSettings appSoundsSettings;
        [SerializeField, IsntNull] SoundSetPlayer soundPlayerPrefab;
        
        static AppSounds instScene;
        static AppSounds instDontDestroyOnLoad;
        AppAudioAccountData data;
        
        Pool<SoundSetPlayer> pool;
        public Dictionary<string, SoundGroup> soundGroups;
  
        public static void SetAsSceneSound(AppSounds appSounds)
        { 
            instScene = appSounds; 
        }
        
        public static void SetAsDontDestroyOnLoadAppSounds(AppSounds appSounds)
        { 
            instDontDestroyOnLoad = appSounds; 
        }
        
        public void Init(AppAudioAccountData data)
        {
            Assert.IsNotNull(data);

            this.data = data;  
            pool = new Pool<SoundSetPlayer>(transform, soundPlayerPrefab);
            pool.FillPool(32);

            soundGroups = new Dictionary<string, SoundGroup>();
            foreach (SoundGroupSettings soundGroupSetting in appSoundsSettings.soundGroupSettings)
                soundGroups.Add(soundGroupSetting.name, new SoundGroup(soundGroupSetting));
        }

        public static SoundSetPlayer GetSoundPlayer(SoundsSet soundsSet)
        {
            return GetInst(soundsSet).GetSoundPlayerInternal(soundsSet);
        }

        SoundSetPlayer GetSoundPlayerInternal(SoundsSet soundsSet)
        {
            SoundSetPlayer soundPlayer = pool.Get();
            soundPlayer.SetSoundSet(this, soundsSet);
            soundPlayer.SetTypeVolume(SoundsVolume);

            return soundPlayer;
        }
         
        public static void Play(SoundsSet soundSet)
        {
            GetSoundPlayer(soundSet).Play();
        }

        public void ReturnToPool(SoundSetPlayer soundPlayer)
        { 
            pool.ReturnToPool(soundPlayer);
        }


        static AppSounds GetInst(SoundsSet soundsSet)
        {
            return soundsSet.dontDestroyOnload ? instDontDestroyOnLoad : instScene;
        }

        public static void OnVolumeChanged()
        {
            if (VolumeChanged != null)
                VolumeChanged.Invoke();
        }

    }
}