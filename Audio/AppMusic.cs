using System.Collections;
using GamePackages.Core;
using UnityEngine;

namespace GamePackages.Audio
{  
    public class AppMusic : MonoBehaviour
    {   
        static AppMusic inst; 
        
        Coroutine volumeDownCoroutine;
        SoundSetPlayer player; 

        public static bool IsPlaying => inst.player.IsPlaying;
        public static SoundSetPlayer Play(SoundsSet soundsSet) => inst.PlayInternal(soundsSet);
        public static void StopWithFade() => inst.StopWithFadeInternal(); 
        public static void StopImmediate() => inst.StopImmediateInternal(); 

    

        public void Init()
        { 
            inst = this; 
        }

        SoundSetPlayer PlayInternal(SoundsSet soundsSet)
        {
            if (volumeDownCoroutine != null)
            {
                StopCoroutine(volumeDownCoroutine);
                volumeDownCoroutine = null;
            }

            if (player)
                player.Stop();

            soundsSet.dontDestroyOnload = true;
            player = AppSounds.GetSoundPlayer(soundsSet);
            player.SetTypeVolume(AppSounds.MusicVolume);
            player.Play();
            return player;
        }

        void StopImmediateInternal()
        {
            if(!player)
                return;
            
            player.Stop();
            player = null;
            
            if (volumeDownCoroutine != null)
                StopCoroutine(volumeDownCoroutine);
        }

        void StopWithFadeInternal()
        { 
            if(!player)
                return;
            
            if (volumeDownCoroutine != null)
                StopCoroutine(volumeDownCoroutine);

            volumeDownCoroutine = StartCoroutine(VolumeDownCoroutine()); 
        }

        IEnumerator VolumeDownCoroutine()
        {
           yield return StartCoroutine(Fade.FromTo(1, 0f, 2, x => player.SetLocalVolume(x), true));
           player.Stop();
           player = null;
        } 
    }
}