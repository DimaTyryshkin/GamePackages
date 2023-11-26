using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GamePackages.Audio
{
    public class SoundOnClick : MonoBehaviour
    {
        [SerializeField, IsntNull]
        SoundsSet sound;

        void Start()
        { 
            
            var btn = GetComponent<Button>();
            Assert.IsTrue(btn);
            
            btn.onClick.AddListener(()=> AppSounds.GetSoundPlayer(sound));
        }
    }
}
