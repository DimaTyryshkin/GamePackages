using GamePackages.Core.Validation;
using System;
using TMPro;
using UnityEngine;

namespace GamePackages.Core
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField, IsntNull] TMP_Text timerText;
        [SerializeField]

        static float startTime;

        private void Update()
        {
            TimeSpan timeInGame = TimeSpan.FromSeconds(GetTime() - startTime);
            timerText.text = timeInGame.ToString(@"mm\:ss");
        }

        float GetTime() => Time.realtimeSinceStartup;

        public void ResetStartTime()
        {
            startTime = GetTime();
        }
    }
}
