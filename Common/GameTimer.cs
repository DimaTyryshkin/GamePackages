using GamePackages.Core.Validation;
using System;
using TMPro;
using UnityEngine;

namespace GamePackages.Core
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField, IsntNull] TMP_Text timerText;

        float startTime;
        private void Start()
        {
            startTime = Time.unscaledTime;
        }

        private void Update()
        {
            TimeSpan timeInGame = TimeSpan.FromSeconds(Time.unscaledTime - startTime);
            timerText.text = timeInGame.ToString(@"mm\:ss");
        }
    }
}
