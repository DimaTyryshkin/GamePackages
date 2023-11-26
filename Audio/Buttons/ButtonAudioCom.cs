using UnityEngine;

namespace GamePackages.Audio
{
    [CreateAssetMenu(fileName = "ButtonAudioCom", menuName = "GamePackages/Audio/ButtonAudioCommon")]
    public class ButtonAudioCom : ScriptableObject
    {
        public SoundsSet onClick;
        public SoundsSet onSelect;
    }
}