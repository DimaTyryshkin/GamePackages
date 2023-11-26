using System;
using System.Collections.Generic;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GamePackages.Core
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField, IsntNull]
        Text fpsText;
        
        Queue<float> framesDeltaTime;
        DateTime lastFrameTime;
        float timeNextGuiUpdate;
        int lastFps;

        void Start()
        {
            lastFrameTime = DateTime.Now;
            framesDeltaTime = new Queue<float>();
        }

        void Update()
        {
            float deltaTime = (float)(DateTime.Now - lastFrameTime).TotalSeconds;
            lastFrameTime = DateTime.Now;
            
            if (framesDeltaTime.Count > 10)
                framesDeltaTime.Dequeue();

            Assert.IsTrue(framesDeltaTime.Count < 11);
            
            framesDeltaTime.Enqueue(deltaTime);

            if (Time.unscaledTime > timeNextGuiUpdate)
            {
                timeNextGuiUpdate = Time.unscaledTime + 1f/3f;
                float averageDeltaTime = Average(framesDeltaTime);
                int newFps = 0;
                if (averageDeltaTime > 0)
                    newFps = Mathf.RoundToInt(1f / averageDeltaTime);

                if (newFps != lastFps)
                {
                    lastFps = newFps;
                    fpsText.text = $"FPS: {newFps}";
                }
            }
        }

        float Average(Queue<float> queue)
        {
            float sum = 0;
            foreach (float frameDeltaTime in queue)
                sum += frameDeltaTime;

            return sum / queue.Count;
        }

        [Button]
        public void SetQualitySettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 0;
        }
        
        [Button]
        public void TargetFps15()
        { 
            Application.targetFrameRate = 15;
            
        }
        
        [Button]
        public void TargetFps60()
        { 
            Application.targetFrameRate = 60;
            Debug.Log("60");
            
        }
        
       
       
        [Button]
               public void TargetFps300()
               { 
                   Application.targetFrameRate = 300;
               }
    }
}
