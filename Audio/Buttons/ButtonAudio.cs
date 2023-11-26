using GamePackages.Core.Validation;
using UnityEngine; 
using UnityEngine.UI;

namespace GamePackages.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : MonoBehaviour
    {
        [SerializeField, IsntNull] ButtonAudioCom com;
        [SerializeField, IsntNull] Button button;

        void Reset()
        {
            button = GetComponent<Button>();
        }

        void Start()
        {
            button.onClick.AddListener(OnClick);
        }
  
        void OnClick()
        {
            Play(com.onClick); 
        }
        
        void Play(SoundsSet soundsSet)
        {
            if(soundsSet)
                AppSounds.Play(soundsSet);
        }
    }
}