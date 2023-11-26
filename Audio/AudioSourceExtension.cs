using UnityEngine;

namespace GamePackages.Audio
{
    public static class AudioSourceExtension
    {
        #region SoundsSet

        public static void Play(this AudioSource audioSource, SoundsSet soundsSet)
        {
            audioSource.Play(soundsSet, 1f);
        }

        public static void Play(this AudioSource audioSource, SoundsSet soundsSet, float volume)
        {
            if (soundsSet)
            {
                audioSource.Play(soundsSet.NextClip(), volume);
            }
        }

        #endregion

        #region AudioClipWrapper 

        public static void Play(this AudioSource audioSource, AudioClipWrapper audioWrapper)
        {
            audioSource.Play(audioWrapper, 1f);
        }

        public static void Play(this AudioSource audioSource, AudioClipWrapper audioWrapper, float volume)
        {
            if (audioWrapper)
            {
                audioSource.LoadDataFromWrapper(audioWrapper);

                float volumeResult = audioWrapper.Volume * volume;
 
                volumeResult *= AppSounds.Volume * AppSounds.SoundsVolume;

                audioSource.volume = volumeResult;

                audioSource.Play();
            }
        }

        public static void LoadDataFromWrapper(this AudioSource audioSource, AudioClipWrapper audioWrapper)
        {
            audioSource.clip   = audioWrapper.AudioClip;
            audioSource.volume = audioWrapper.Volume;
            audioSource.pitch  = audioWrapper.PitchRandom; 
        }

        #endregion
    }
}