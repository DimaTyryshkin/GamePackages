using System.Linq;
using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;

using UnityEditor;

namespace GamePackages.Audio
{
    public class AudioWrappersBrowser : EditorWindow
    {
        readonly AudioGUI drawer = new AudioGUI();

        public static readonly string createNewGroupLabel = "Create new";
        DirectoryTreeBuilder<SoundsSet> directoryBuilder;
        
        string[] groups; 
        MusicsSet[]        musicSets;
        AudioClipWrapper[] wrappersFree;
        AudioClip[]        audioClipsFree;

        Vector2 scroll;
        int pageIndex = 1;

        //int  columnSizeName    = 200; 
        //int  columnSizeToggle  = 20 * 3;
        //int  columnSizePlayBtn = 50;
        //int  columnSizeVolime  = 100;
        //int  columnSizePitch   = 30 + 30;
        //readonly int  leftTab = 30;
        //readonly int  offset = 20;
        readonly string[] pages = new string[]
        {
            "SoundSets",
            "Sets",
            "Wrappers",
            "Clips"
        };

        bool wrappersIsFoldOut;
        bool clipsIsFoldOut;
            
        bool isDoubleError;
        bool showHelp;
         
        
        public static readonly int columnSizePlayBtn = 50;
        public static AppSoundsSettings appSoundsSettings;
        public static GUIStyle buttonLeftTextAlignmentStyle;
        GUIStyle headerStyle;
        GUIStyle foldHeaderStyle;

        [MenuItem("GamePackages/Audio/Browser")]
        static void Init()
        { 
            AudioWrappersBrowser w = (AudioWrappersBrowser) GetWindow(typeof(AudioWrappersBrowser));
            //w.Load();
            w.titleContent = new GUIContent("Sounds browser");
            w.Show();
        }

        void OnGUI()
        {
            Validate();

            Draw();
        }

        void Validate()
        {
            if (buttonLeftTextAlignmentStyle == null ||
                directoryBuilder == null ||
                !appSoundsSettings)
                Load();
            
            if(wrappersFree.Any(x=>!x) ||
               audioClipsFree.Any(x=>!x) ||
               !directoryBuilder.rootDirectory.Validate())
                Load();
        }

        void Draw()
        {
            GUILayout.BeginVertical();
            { 
                GUILayout.BeginHorizontal();
                {
                    pageIndex = GUILayout.Toolbar(pageIndex, pages);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
 
            scroll = EditorGUILayout.BeginScrollView(scroll);
            { 
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                { 
                    //MusicSets
                    // if (pageIndex == 0)
                    // {
                    //     foreach (var w in musicSets)
                    //     {
                    //         drawer.DrawDirectory(w, groups, 0, 0); 
                    //         GUILayout.Space(10);
                    //     }
                    // }
                    
                    // Sets
                    if (pageIndex == 1)
                    {
                        drawer.DrawDirectory(directoryBuilder.rootDirectory, groups, 0);
                    }
                    
                    // Wrappers
                    if (pageIndex == 2)
                    {
                        foreach (var w in wrappersFree)
                        {
                            drawer.DrawWrapper(w, groups, 0, 0); 
                            GUILayout.Space(10);
                        }
                    }
                    
                    // Audio Clips
                    if (pageIndex == 3)
                    {
                        foreach (var clip in audioClipsFree)
                        {
                            drawer.DrawAudioClip(clip, 0, Reload);
                          
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ReLoad"))
                    Reload();

                if (GUILayout.Button("Stop", GUILayout.Width(columnSizePlayBtn)))
                    EditorAudioExtensions.Stop();
            }
            GUILayout.EndHorizontal();
        }

        void DrawCatchWrapper()
        { 
            try
            {
                Draw();
                isDoubleError = false;
                
            }
            catch (System.Exception e)
            {
                if (!isDoubleError)
                {
                    isDoubleError = true;
                    Reload();
                }
                else
                {
                    Debug.LogError(e);
                    Close();
                }
            }
        }

        void Reload()
        {
            Load();
            Repaint();
        }

        /// <summary>
        /// Грузим структуру папок и все SoundSet и AudioClipWrapper
        /// находим AudioClip и AudioClipWrapper которые нигде не используются 
        /// отделяем MusicsSet от SoundSet
        /// </summary>
        void Load()
        { 
            buttonLeftTextAlignmentStyle =  new GUIStyle(GUI.skin.button);
            buttonLeftTextAlignmentStyle.alignment = TextAnchor.MiddleLeft;
            
            Color color = EditorGUIUtility.isProSkin ? Color.green : Color.blue;
            headerStyle = drawer.GetGUIForFoldLabel(FontStyle.Bold, color, false);
            
            foldHeaderStyle = drawer.GetGUIForFoldLabel(FontStyle.Bold, Color.red, true);
            
            directoryBuilder = new DirectoryTreeBuilder<SoundsSet>();
            directoryBuilder.LoadAllFiles("Assets/Sounds/Sets");
            
            //Clips free
            var allClip = EditorExtension.LoadAllAssetsOfType<AudioClip>("Assets/Sounds", includeSubFolder: true);
            HashSet<AudioClip> unusfulClips = new HashSet<AudioClip>(allClip);
            
            //Wrappers
            var allWrappers = EditorExtension.LoadAllAssetsOfType<AudioClipWrapper>("Assets/Sounds", includeSubFolder: true);
            foreach (var wrap in allWrappers)
            {
                wrap.useCount = 0;
                unusfulClips.Remove(wrap.AudioClip); //Удаялем использлванные клипы
            }
            audioClipsFree = unusfulClips.OrderBy(c => c.name).ToArray();
            
            //Load SoundGroups
            appSoundsSettings = AssetDatabase.LoadAssetAtPath<AppSoundsSettings>("Assets/Sounds/AppSoundsSettings.asset");
            Assert.IsNotNull(appSoundsSettings);
            var groupsData = new List<string> {"None"};
            groupsData.AddRange(appSoundsSettings.soundGroupSettings.Select(x => x.name));
            groupsData.Add(createNewGroupLabel);
            groups = groupsData.ToArray();

            //Sound sets
            List<SoundsSet> allSoundSets = EditorExtension.LoadAllAssetsOfType<SoundsSet>("Assets/Sounds", includeSubFolder: true).ToList();
            HashSet<AudioClipWrapper> unusfulWrappers = new HashSet<AudioClipWrapper>(allWrappers);
            foreach (var set in allSoundSets)
            {
                if (set.clips != null)
                {
                    foreach (AudioClipWrapper wrapper in set.clips)
                    {
                        if (wrapper)
                        {
                            wrapper.useCount++; //тут подсчет заодно
                            EditorUtility.SetDirty(wrapper);
                            unusfulWrappers.Remove(wrapper);
                        }
                    }
                }
            }
            
            wrappersFree = unusfulWrappers.OrderBy(c => c.name).ToArray();
            
            musicSets = allSoundSets.Select(x => x)
                .Where(x => x is MusicsSet)
                .Cast<MusicsSet>()
                .OrderBy(x => x.name)
                .ToArray();
            
            foreach (var set in musicSets)
            {
                allSoundSets.Remove(set);
            } 
        }
    }
}