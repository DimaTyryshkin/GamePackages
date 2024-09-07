#if UNITY_EDITOR

using System;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GamePackages.Core
{
    public class EditorCoroutine
    {
        /// <summary>
        /// Use only "yield return null" to skip frame in editor
        /// </summary> 
        /// <param name="routineContainer">corotine still live while <paramref name="routineContainer"/> still exist </param>
        /// <returns></returns>
        public static EditorCoroutine Start(IEnumerator routine, UnityEngine.Object routineContainer = null)
        { 
            EditorCoroutine coroutine = new EditorCoroutine(routine, routineContainer);
            coroutine.Start();
            return coroutine;
        }

        public static EditorCoroutine CallWithDelay(UnityAction action, float delay, UnityEngine.Object routineContainer = null)
        {
            return Start(Delay(delay, action));
        }

        CustomCoroutineHandler routineHandler;
        Object      routineContainer;
        bool        routineContainerRequired; 

        EditorCoroutine(IEnumerator routine, Object routineContainer = null)
        {
            this.routineContainer    = routineContainer;
            routineContainerRequired = routineContainer;
            
            routineHandler = new CustomCoroutineHandler(routine); 
        }

        void Start()
        {
            EditorApplication.update += Update;
        }

        public void Stop()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (!routineContainerRequired || routineContainer)
            {
                if (!routineHandler.MoveNext())
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }

        static IEnumerator Delay(float delay, UnityAction action)
        {
            DateTime t = DateTime.Now + TimeSpan.FromSeconds(delay);
            while (DateTime.Now < t)
                yield return null;
            
            action?.Invoke();
        }
    }
}
#endif