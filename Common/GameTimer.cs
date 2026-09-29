using GamePackages.Core.Validation;
using System;
using TMPro;
using UnityEngine;

namespace GamePackages.Core
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField, IsntNull] TMP_Text timerText;

        private void Update()
        {
            TimeSpan timeInGame = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
            timerText.text = timeInGame.ToString(@"mm\:ss");
        }
    }
}
