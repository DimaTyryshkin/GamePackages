using UnityEngine;
using System;
using GamePackages.Core.Validation;
using UnityEngine.Audio;

namespace GamePackages.Audio
{  
    [CreateAssetMenu(fileName = "SoundsSet", menuName = "GamePackages/Audio/SoundsSet")]
    public class SoundsSet : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Открыто ли в браузере звуков
        /// </summary>
        public bool IsFoldOutOnBrowser { get; set; }

        public bool isAdded;

        [HideInInspector]
        public bool isDone;

        [HideInInspector]
        public bool isBug;

        [TextArea(3, 10)]
        public string info;
#endif

        //public float spatialBlend;
        public string soundGroup;
        [SerializeField, IsntNull] AudioMixerGroup audioMixerGroup;
        public bool dontDestroyOnload;
        
        //[SerializeField]
        public AudioClipWrapper[] clips;

        [NonSerialized]
        int lastClipIndex;

        public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

        public AudioClipWrapper NextClip()
        {
            if (clips.Length == 0)
                return null;

            if (clips.Length == 1)
                return clips[0];

            if (clips.Length == 2)
                return clips[UnityEngine.Random.Range(0, 2)];

            int index = UnityEngine.Random.Range(0, clips.Length);
            if (lastClipIndex == index)
                index = (index + 1) % clips.Length;

            lastClipIndex = index;
            return clips[index];
        }

        //public void PlayNextClip(AudioSource audioSource)
        //{
        //    Play(audioSource, Profile.instance.getSounds() * Profile.instance.getVolume());
        //}

        //void Play(AudioSource audioSource, float volume)
        //{
        //    AudioClipWrapper clip = NextClip();

        //    if (clip != null)
        //    {
        //        if (audioSource.isPlaying == false)
        //        {
        //            audioSource.volume = volume;
        //            audioSource.clip = clip;
        //            audioSource.Play();
        //        }
        //    }
        //}
    }
}