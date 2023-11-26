using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace GamePackages.Audio
{
    public static class EditorAudioExtensions
    {
        static int highlighterCount;
        static AudioSource lastAudioSource;
        static EditorCoroutine growSoundCoroutine;
        static bool pitchFlip;

        static EditorAudioExtensions()
        {
            EditorApplication.update += HighlighterStop;
        }
        [MenuItem("GamePackages/Audio/Find Sound Sets/Select next SoundSet field Key")]
        public static void SelectNextSoundSetField_Editor()
        {
            SelectNextSoundSetFieldOnScene(true);
        }

        [MenuItem("GamePackages/Audio/Find Sound Sets/Select next SoundSet field in assets Key")]
        public static void SelectNextSoundSetFieldInAssets_Editor()
        {
            SelectNextSoundSetFieldOnScene(false);
        }

        static void HighlighterStop()
        {
            highlighterCount--;
            if (highlighterCount < 0)
            {
                highlighterCount = 0;
                Highlighter.Stop();
            }
        }

        [MenuItem("GamePackages/Audio/Find Sound Sets/Reset findlist")]
        public static void SelectNextSoundSetFieldClear_Editor()
        {
            objList = null;
        }
        
        [MenuItem("Assets/GamePackages/CreateAudioWrapper")]
        private static void CreateAudioWrapper()
        {
            CreateWrapper(Selection.activeObject as AudioClip);
        }
        
        [MenuItem("Assets/GamePackages/CreateAudioWrapper", true)]
        private static bool CreateAudioWrapperValidation()
        {
            return Selection.activeObject is AudioClip;
        }

        public static void PlayClip(AudioClipWrapper wrapper,  AudioMixerGroup audioMixerGroup, float fadeIn = 0)
        {
            if(!wrapper)
                return;
            
            // В редакторе проигрываем по очереди максимыльный и минимальный питч.
            float pitch = pitchFlip ? 
                wrapper.pitch.min : 
                wrapper.pitch.max;

            pitchFlip = !pitchFlip;
            
            PlayClip(wrapper.AudioClip, wrapper.Volume, pitch, audioMixerGroup, wrapper.startTime, fadeIn);
        }

        public static AudioSource PlayClip(AudioClip audioClip, float volume, float pitch,  AudioMixerGroup audioMixerGroup, float start = 0, float fadeIn = 0)
        {
            Stop();

            var gameObject = new GameObject("AudioSource", typeof(AudioSource));
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            lastAudioSource = gameObject.GetComponent<AudioSource>();

            lastAudioSource.outputAudioMixerGroup = audioMixerGroup;
            lastAudioSource.clip   = audioClip;
            lastAudioSource.volume = volume;
            lastAudioSource.pitch  = pitch;
            lastAudioSource.time   = start;
            lastAudioSource.Play();
            
            if (growSoundCoroutine != null)
            {
                growSoundCoroutine.Stop();
                growSoundCoroutine = null;
            }

            if(fadeIn > 0)
                growSoundCoroutine = EditorCoroutine.Start(VolumeGrow(lastAudioSource, volume, fadeIn));

            return lastAudioSource;
        }

        public static void Stop()
        {
            if (lastAudioSource)
            {
                lastAudioSource.Stop();
                Object.DestroyImmediate(lastAudioSource.gameObject);
            }
        }
 
        static IEnumerator VolumeGrow(AudioSource audioSource,float _volumeTarget, float time)
        {
            float defaultVolume = 0;
            float volume = 0;

            DateTime beginGrowTime = DateTime.Now;
            DateTime targetGrowTime = DateTime.Now + TimeSpan.FromSeconds(time);
            
            var targetTicks = targetGrowTime.Ticks - beginGrowTime.Ticks;
            
            while (DateTime.Now < targetGrowTime)
            {
                audioSource.volume = volume;
                yield return null;
                
                long currentTicks = DateTime.Now.Ticks - beginGrowTime.Ticks;
                volume = Mathf.Lerp(defaultVolume, _volumeTarget, (float)currentTicks/targetTicks);
            }
            audioSource.volume = _volumeTarget;
        }

        #region AudioWrapper

        [MenuItem("GamePackages/Audio/CreateWrappers", validate = true)]
        public static bool CreateWrappersValidate()
        {
            foreach (var item in Selection.objects)
            {
                if (item as AudioClip) return true;
            }

            return false;
        }

        [MenuItem("GamePackages/Audio/CreateWrappers")]
        public static void CreateWrappers()
        {
            foreach (var item in Selection.objects)
            {
                var clip = item as AudioClip;
                if (clip) CreateWrapper(clip);
            }

            Debug.Log("YTools/Audio/CreateWrappers done");
        }

        public static void CreateWrapper(AudioClip audioClip)
        {
            if (audioClip)
            {
                var path = AssetDatabase.GetAssetPath(audioClip);

                if (path.Contains("Assets"))
                {
                    path = path.Replace(Path.GetExtension(path), ".asset");

                    var oldWrapper = AssetDatabase.LoadAssetAtPath<AudioClipWrapper>(path);

                    if (!oldWrapper)
                    {
                        var newWrapepr = ScriptableObject.CreateInstance<AudioClipWrapper>();
                        newWrapepr.AudioClip = audioClip;

                        AssetDatabase.CreateAsset(newWrapepr, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        #endregion

        #region Select next SoundSet field

        static List<KeyValuePair<UnityEngine.Object, string>> objList;
        static int                                            curIndex;

        static Type[] exeptionsTypes = new Type[]
        {
            // typeof(WindowStackElement),
            // typeof(Bullet)
        };

        public static void SelectNextSoundSetFieldOnScene(bool loadFromScene)
        {
            if (objList == null)
            {
                Debug.Log("--------Start---------");
                curIndex = 0;
                objList  = new List<KeyValuePair<UnityEngine.Object, string>>(128);

                if (loadFromScene)
                {
                    //Fimnd all
                    foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    {
                        go.transform.ForAllHierarchy(t => ProssesObject(t.gameObject));
                    }
                }
                else
                {
                    //Load from assets
                    //var allAssets = AssetDatabase.FindAssets("t:GameObject");
                    //Debug.Log("Count1=" + allAssets.Length);

                    var allScriptableObject = EditorExtension.LoadAllAssetsOfType<UnityEngine.ScriptableObject>();
                    foreach (var item in allScriptableObject)
                    {
                        ProssesObject(item);
                    }

                    var allGameObject = EditorExtension.LoadAllAssetsOfType<UnityEngine.GameObject>();
                    foreach (var item in allGameObject)
                    {
                        ProssesObject(item);
                    }


                    //foreach (var asset in allAssets)
                    //{
                    //    ProssesObject(asset);
                    //}
                }

                Debug.Log("Count=" + objList.Count);
            }

            if (curIndex < objList.Count)
            {
                Debug.Log(curIndex.ToString() + "/" + (objList.Count - 1).ToString() + " " + objList[curIndex].Value);
                Selection.activeObject = objList[curIndex].Key;

                //Highlighter.Highlight("Inspector", objList[curIndex].Value); 
                //Highlighter.Highlight("Inspector", "color");

                highlighterCount = 100;

                curIndex++;
            }
            else
            {
                objList = null;
                Debug.Log("--------End---------");
            }
        }


        static void ProssesObject(UnityEngine.Object obj)
        {
            string path = GetFieldPath(obj, 1);

            if (path != null)
            {
                objList.Add(new KeyValuePair<UnityEngine.Object, string>(obj, path));
            }
        }

        /// <summary>
        /// Ищит в объекте(в коспонентах и вложенных структурах) пустые поля орпеделенного типа. 
        /// </summary> 
        static string GetFieldPath(System.Object obj, int depth)
        {
            if (depth > 3) return null;
            //Debug.Log("depth=" + depth);

            if (obj.GetType() == typeof(GameObject))
            {
                //if (((GameObject)obj).name == "Towers")
                //{
                //    obj = obj;
                //}

                var allComp    = ((GameObject) obj).GetComponents<MonoBehaviour>();
                var componenst = allComp.Where(c => !exeptionsTypes.Contains(c.GetType()));

                foreach (var c in componenst)
                {
                    //Debug.Log(((GameObject)obj).name + " " + c.name);

                    string path = GetFieldPath(c, depth + 1);
                    if (path != null) return path;
                }
            }
            else
            {
                IEnumerable<FieldInfo> allFields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                ////Отсекаем дл яизбежания бесконечной рекурсии
                //if (obj.GetType().IsClass == false)
                //{
                //    if (allFields.Count() < 2)
                //    {

                //    }
                //}

                foreach (var f in allFields)
                {
                    //------Вот собственно условие выбора
                    if (f.FieldType == typeof(SoundsSet))
                    {
                        if (f.GetValue(obj) == null)
                        {
                            //if (f.Name == "bayAudioClip")
                            //{
                            //    int a = 1;
                            //}

                            if (f.IsPrivate)
                            {
                                if (f.GetCustomAttributes(typeof(SerializeField), inherit: false).Length > 0)
                                {
                                    Debug.Log(f.Name);
                                    return f.Name;
                                }
                            }
                            else
                            {
                                Debug.Log(f.Name);
                                return f.Name;
                            }
                        }
                    }
                }

                //Проверим нет ли структуры в котолрой есть поле SoundSet
                var allStructFields = allFields.Where(f => f.FieldType.IsClass == false);

                foreach (var sf in allStructFields)
                {
                    //Debug.Log(sf.Name);
                    var value = sf.GetValue(obj);
                    if (value != null)
                    {
                        string path = GetFieldPath(value, depth + 1);
                        if (path != null) return sf.Name + "." + path;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}