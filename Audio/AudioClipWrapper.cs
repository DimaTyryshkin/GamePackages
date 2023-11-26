using GamePackages.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace GamePackages.Audio
{
    public class AudioClipWrapper : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Колличество использований в сетах. Подсчитывается в браузере звуков
        /// </summary>
        [HideInInspector]
        public int useCount;
#endif
        
        [SerializeField]
        AudioClip audioClip;

        [SerializeField]
        [Range(0f, 1f)]
        float volume = 0.5f;
        
        public float startTime = 0f;

        public RangeMinMax pitch = new RangeMinMax(1, 1);
 
        public AudioClip AudioClip
        {
            get => audioClip;
            set => audioClip = value;
        }

        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp01(value);
        }

        public float PitchRandom => pitch.Random(); 
    }
}