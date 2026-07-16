using GamePackages.Core.Validation;
using System;
using TMPro;
using UnityEngine;

namespace GamePackages.Core
{
    public class RealTimeCounter : MonoBehaviour
    {
        [SerializeField, IsntNull]
        TMP_Text fpsText;

        private void LateUpdate()
        {
            fpsText.text = TimeSpan.FromSeconds(Time.realtimeSinceStartup).ToString(@"mm\:ss");
        }
    }
}
