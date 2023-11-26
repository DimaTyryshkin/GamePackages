using System.Collections.Generic;
using GamePackages.Common;
using GamePackages.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GamePackages.Audio
{  
    public class AudioGUI
    {
        int offsetSize = 30; 
        int toggleSize = 20; 
        int columnSizeVolume = 100;
        
        public void DrawDirectory(EditorDirectory<SoundsSet> directory, string[] groups, int offsetStep)
        {
            var color = EditorGUIUtility.isProSkin? Color.yellow: Color.magenta;
            var directoryStyle = GetGUIForFoldLabel(FontStyle.Bold, color, true);
            Dictionary<string, EditorDirectory<SoundsSet>> directories = directory.directories;
            foreach (var dirInfo in directories)
            {
                EditorDirectory<SoundsSet> curDirectory = dirInfo.Value;
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    GUILayout.Space(offsetSize * offsetStep);
                    curDirectory.IsFoldOutOnBrouser = EditorGUILayout.Foldout(curDirectory.IsFoldOutOnBrouser, dirInfo.Key, true, directoryStyle);
                }
                GUILayout.EndHorizontal();
                
                if (curDirectory.IsFoldOutOnBrouser)
                {
                    DrawDirectory(curDirectory, groups,offsetStep+1);
                }
            }
            
            foreach (var file in directory.files)
            {
                DrawSoundSet(file, groups, offsetStep);
            }
        }

        public void DrawSoundSet(SoundsSet set, string[] groups, int offsetStep)
        {
            int offset = offsetSize * offsetStep;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(offset);
                var color = EditorGUIUtility.isProSkin? Color.green: Color.blue;
                var soundStyle = GetGUIForFoldLabel(FontStyle.Normal, color, true);
                set.IsFoldOutOnBrowser = EditorGUILayout.Foldout(set.IsFoldOutOnBrowser, set.name, true, soundStyle);
                //set.IsFoldOutOnBrouser = EditorGUILayout.Foldout(set.IsFoldOutOnBrouser, set.name, true, AudioWrappersBrowser.buttonLeftTextAlignmentStyle);
                GUILayout.FlexibleSpace();
                 
                var groupIndex = groups.IndexOf(set.soundGroup);
                if (groupIndex == -1)
                {
                    //set.soundGroup = "None";
                    //groupIndex = 0;
                }
                    
                groupIndex = EditorGUILayout.Popup(groupIndex, groups, GUILayout.Width(100));
                if (groupIndex == groups.Length - 1)
                {
                    Selection.activeObject = AudioWrappersBrowser.appSoundsSettings;
                }
                else
                {
                    if (groupIndex >= 0)
                        set.soundGroup = groups[groupIndex];
                }

                set.isAdded = GUILayout.Toggle(set.isAdded, "", GUILayout.Width(toggleSize));
                set.isDone = GUILayout.Toggle(set.isDone, "", GUILayout.Width(toggleSize));
                set.isBug = GUILayout.Toggle(set.isBug, "", GUILayout.Width(toggleSize));
                
                if (GUILayout.Button(" Open", GUILayout.Width(AudioWrappersBrowser.columnSizePlayBtn)))
                    SelectObject(set);
                
                if (GUILayout.Button("Play", GUILayout.Width(AudioWrappersBrowser.columnSizePlayBtn)))
                    EditorAudioExtensions.PlayClip(set.NextClip(), set.AudioMixerGroup); 
            }
            GUILayout.EndHorizontal();

            if (set.IsFoldOutOnBrowser)
            {
                if (!string.IsNullOrEmpty(set.info))
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(offset + offsetSize);
                        GUI.enabled = false;
                        GUILayout.TextArea(set.info);
                        GUI.enabled = true;
                        GUILayout.Space(AudioWrappersBrowser.columnSizePlayBtn);
                    }
                    GUILayout.EndHorizontal();
                }
                
                if (set.clips.Length!=0)
                {
                    foreach (var wrapper in set.clips)
                    {
                        if (wrapper)
                        {
                            float fadeIn = 0;
                            if (set is MusicsSet musicSet)
                                fadeIn = musicSet.fadeInTime;

                            GUILayout.BeginHorizontal();
                            {
                                DrawWrapper(wrapper, groups, fadeIn, offset + offsetSize);
                                GUILayout.Space(AudioWrappersBrowser.columnSizePlayBtn + 3);
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                            GUILayout.Label("Null");
                    }

                    if (set is MusicsSet musicsSet)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(offset);
                            GUILayout.Label("FadeIn: 0", GUILayout.Width(55));
                            musicsSet.fadeInTime = GUILayout.HorizontalSlider(musicsSet.fadeInTime, 0, 5, GUILayout.Width(columnSizeVolume));
                            GUILayout.Label("5сек.  ", GUILayout.Width(40));
                            musicsSet.fadeInTime = EditorGUILayout.FloatField(musicsSet.fadeInTime, GUILayout.Width(35));
                            EditorGUI.BeginChangeCheck();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
        
        

        public void DrawAudioClip(AudioClip clip, int offset, UnityAction callbackOnCreateWrapper)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(offset);
                if (GUILayout.Button(clip.name, AudioWrappersBrowser.buttonLeftTextAlignmentStyle, GUILayout.ExpandWidth(true)))
                    SelectObject(clip);  
                
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    EditorAudioExtensions.CreateWrapper(clip);
                    callbackOnCreateWrapper?.Invoke();
                }
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("Play", GUILayout.Width(AudioWrappersBrowser.columnSizePlayBtn)))
                    EditorAudioExtensions.PlayClip(clip,  1f, 1f,null);
            }
            GUILayout.EndHorizontal();
        }

        public void DrawWrapper(AudioClipWrapper wrapper, string[] groups, float fadeIn = 0, int offset = 0)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                { 
                    GUILayout.Space(offset);
                    if (GUILayout.Button(wrapper.name, AudioWrappersBrowser.buttonLeftTextAlignmentStyle, GUILayout.ExpandWidth(true)))
                    {
                        SelectObject(wrapper);
                    }
                    
                    if (wrapper.useCount > 1)
                        GUILayout.Label(wrapper.useCount.ToString(), GUILayout.Width(25));
                    
                    if (GUILayout.Button("Clip", GUILayout.Width(AudioWrappersBrowser.columnSizePlayBtn)))
                        SelectObject(wrapper.AudioClip);
   
                    if (GUILayout.Button("Play", GUILayout.Width(AudioWrappersBrowser.columnSizePlayBtn - 3)))
                        EditorAudioExtensions.PlayClip(wrapper, null, fadeIn);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                {  
                    GUILayout.Space(offset);
                    EditorGUILayout.MinMaxSlider(
                        "Pitch", 
                        ref  wrapper.pitch.min , 
                        ref  wrapper.pitch.max, 
                        0.1f,
                        2f, 
                        GUILayout.ExpandWidth(true));
                     
                    wrapper.pitch.min = EditorGUILayout.FloatField(wrapper.pitch.min, GUILayout.Width(40)); 
                    wrapper.pitch.max = EditorGUILayout.FloatField(wrapper.pitch.max,GUILayout.Width(40));
                    
                    if (GUILayout.Button("Reset", GUILayout.Width(50)))
                    {
                        wrapper.pitch.min = 1;
                        wrapper.pitch.max = 1;
                    } 
                    
                    GUILayout.Space(AudioWrappersBrowser.columnSizePlayBtn);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(offset);
                    wrapper.Volume = EditorGUILayout.Slider("Volume", wrapper.Volume, 0, 1);
                    GUILayout.Space(AudioWrappersBrowser.columnSizePlayBtn);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(offset);
                    wrapper.startTime = EditorGUILayout.Slider("Start", wrapper.startTime, 0, 1);
                    GUILayout.Space(AudioWrappersBrowser.columnSizePlayBtn);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(wrapper);
            }
        }
        
        public GUIStyle GetGUIForFoldLabel(FontStyle font, Color color, bool isFold)
        {
            GUIStyle guiStyle = isFold? new GUIStyle(EditorStyles.foldout): new GUIStyle();
            guiStyle.fontStyle = font;
            guiStyle.normal.textColor = color;
            guiStyle.onNormal.textColor = color;
            return guiStyle;
        }
        
        public void SelectObject(Object obj)
        {
            Selection.activeObject = obj;
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }
}