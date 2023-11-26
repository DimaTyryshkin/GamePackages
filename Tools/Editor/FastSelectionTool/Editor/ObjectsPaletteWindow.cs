using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamePackages.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Tools.ObjectsPalette
{
	public class ObjectsPaletteWindow : EditorWindow
	{
		static readonly string assetPath = "ObjectsPalette";
		static readonly string bsdKey = "GamePackages.Tools.ObjectsPalette.ObjectsPaletteWindow.SavedSelectedPalette";

		int SavedSelectedPalette
		{
			get => EditorPrefs.GetInt(bsdKey);
			set => EditorPrefs.SetInt(bsdKey, value);
		}

		string[] palettesNames;
		ObjectsPalette[] palettes;
		ObjectsPalette scenesPalette;
		Dictionary<Object,Texture>  objectToTexture;
		
		int selectedPalette; 
		Vector2 scrollPosition; 
		bool isDrag;
		bool isEditMode;
		
		
		int cellSize;
		readonly int space = 2;

		[MenuItem("GamePackages/Tools/ObjectsPalette")]
		public static void Init()
		{
			ObjectsPaletteWindow window = GetWindow<ObjectsPaletteWindow>();
			window.titleContent.text = "Objects palette";
		}

		void Validate()
		{
			if (palettes == null || palettesNames == null)
				Reload();

			if (palettesNames.Length != palettes.Length)
				Reload();

			if (palettes.Length == 0)
				Reload();

			selectedPalette = Mathf.Clamp(selectedPalette, 0, palettes.Length - 1);

			if (objectToTexture == null)
				objectToTexture = new Dictionary<Object, Texture>();
		}

		void ResetTextures()
		{
			if (objectToTexture != null)
			{
				foreach (var  texture in objectToTexture.Values)
				{
					DestroyImmediate(texture);
				}
			}

			objectToTexture = new Dictionary<Object, Texture>();
		}

		void OnGUI()
		{
			Validate();
			ObjectsPalette palette = palettes[selectedPalette];

			GUILayout.BeginHorizontal();
			{
				selectedPalette = EditorGUILayout.Popup(selectedPalette, palettesNames);
				if (GUI.changed)
					SavedSelectedPalette = selectedPalette;
				
				if(scenesPalette)
					if (GUILayout.Button("Scenes", GUILayout.Width(100)))
					{
						selectedPalette = palettes.IndexOf(scenesPalette);
						SavedSelectedPalette = selectedPalette;
					}


				GUI.color = isEditMode ? Color.red : Color.white;
				if (GUILayout.Button("E", GUILayout.Width(30)))
				{
					isEditMode = !isEditMode;
				}

				GUI.color = Color.white;

				if (GUILayout.Button("RE", GUILayout.Width(30)))
				{
					int oldPalette = selectedPalette;
					Reload();
					selectedPalette = oldPalette;
					Validate();
				}

				if (GUILayout.Button("P", GUILayout.Width(30)))
				{
					Selection.activeObject = palette;
				}
			}
			GUILayout.EndHorizontal();

			if (palette.isLevelDesignPalette)
				DrawLevelDesignPalette(palette);
			else
				DrawBasicPalette(palette);

			if (palette.isLevelDesignPalette && isEditMode)
			{
				GUILayout.BeginHorizontal();
				{
					GUI.color = Color.red;
					GUILayout.Box("Edit mode - Click to remove", GUILayout.ExpandWidth(true));
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
			}

			if (!palette.isLevelDesignPalette || (palette.objects.Count == 0 && palette.subPalettes.Count == 0))
				OnGuiDropArea(palette);
		}

		void OnGuiDropArea(ObjectsPalette palette)
		{
			bool canDrop = DragAndDrop.objectReferences.Length > 0 && (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length);
			float dropAreaHeight = canDrop ? 100 : 20;

			Rect dropRect = GUILayoutUtility.GetRect(0, dropAreaHeight, GUILayout.ExpandWidth(true));
			GUI.Box(dropRect, "Drag and Drop Prefabs to this Box!");
			if (dropRect.Contains(Event.current.mousePosition))
			{
				if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length) // Ассеты из папки Assets
				{
					if (Event.current.type == EventType.DragUpdated)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						Event.current.Use();
					}
					else if (Event.current.type == EventType.DragPerform)
					{
						Undo.RecordObject(palette, "drop objects");
						foreach (var obj in DragAndDrop.objectReferences)
						{
							if (!palette.objects.Contains(obj))
								palette.objects.Add(obj);
						}

						EditorUtility.SetDirty(palette);
						Event.current.Use();

						DragAndDrop.AcceptDrag();
						DragAndDrop.PrepareStartDrag();
					}
				}
			}
		}

		void DrawBasicPalette(ObjectsPalette palette)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			{
				foreach (var obj in palette.objects)
				{
					if (obj)
					{
						if (obj is SceneAsset scene)
						{
							Color c = GUI.color;
							GUI.color = Color.gray;
							if (GUILayout.Button(obj.name, GUILayout.ExpandWidth(true)))
							{
								var pathToScene = AssetDatabase.GetAssetPath(scene);
								if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
									EditorSceneManager.OpenScene(pathToScene);
							}

							GUI.color = c;
						}
						else
						{
							if (GUILayout.Button(obj.name, GUILayout.ExpandWidth(true)))
								//Selection.activeObject = obj;
								AssetDatabase.OpenAsset(obj);
						}
					}
				}
			}
			GUILayout.EndScrollView();
		}


		void DrawLevelDesignPalette(ObjectsPalette palette)
		{
			cellSize = 100;
			
			int width = (int)rootVisualElement.layout.width - 13;
			
			int countOnLine = width / (cellSize + space);
			if (countOnLine == 0)
				countOnLine = 1;
			
			int freePixels = width - countOnLine * (cellSize + space);
			if (freePixels > 0)
			{
				int add = freePixels / countOnLine;
				cellSize += add;
			}


			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			{
				GUILayout.BeginVertical();
				{
					DrawLevelDesignPaletteRecursive(palette, countOnLine);
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}

		void DrawLevelDesignPaletteRecursive(ObjectsPalette palette, int countOnLine)
		{ 
			Assert.IsFalse(palette.subPalettes.Count > 0 && palette.objects.Count > 0, "subPalettes.Count > 0 && objects.Count > 0");

			if (palette.subPalettes.Count == 0)
			{
				DrawPaletteObjects(palette, countOnLine, cellSize, space);
			}
			else
			{
				foreach (var subPalette in palette.subPalettes)
				{
					GUILayout.Label(subPalette.name);
					DrawLevelDesignPaletteRecursive(subPalette, countOnLine);
				}
			}
		}

		void DrawPaletteObjects(ObjectsPalette palette, int countOnLine, int cellSize, int space)
		{
			int i = 0;
			int y = 0;

			while (i < palette.objects.Count)
			{
				GUILayout.BeginHorizontal();
				{
					for (int x = 0; x < countOnLine && i < palette.objects.Count; x++)
					{
						Object obj = palette.objects[i];
						if (obj)
						{
							bool isClick = DrawLevelDesignPaletteItem(palette, obj, cellSize);
							if (isClick)
							{
								if (isEditMode)
								{
									//Undo.RecordObject(palette, "remove item");
									palette.Remove(obj);
									//EditorUtility.SetDirty(palette);
								}
								else
								{
									Selection.activeObject = obj;
								}
							}

							GUILayout.Space(space);
						}

						i++;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(space);
				y++;
			}
		}

		bool DrawLevelDesignPaletteItem(ObjectsPalette palette, Object obj, int cellSize)
		{
			var rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false));

			bool isClick = false;

			if (rect.Contains(Event.current.mousePosition))
			{
				if (Event.current.type == EventType.MouseDown)
				{
					isDrag = false;
					DragAndDrop.PrepareStartDrag();
					DragAndDrop.objectReferences = new[] { obj };
					Event.current.Use();
				}

				if (Event.current.type == EventType.MouseDrag)
				{
					isDrag = true;
					if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] == obj)
					{
						DragAndDrop.StartDrag("Go");
						Event.current.Use();
					}
				}

				if (Event.current.type == EventType.MouseUp)
				{
					if (!isDrag)
					{
						isClick = true;
					}


					isDrag = false;
					Event.current.Use();
				}
				
				bool isReplace = DragAndDrop.objectReferences.Length == 1 && palette.objects.Contains(DragAndDrop.objectReferences[0]);
				bool dragFromAssets = DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length;
				if (isReplace || dragFromAssets) 
				{
					if (Event.current.type == EventType.DragUpdated)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						Event.current.Use();
					}
					else if (Event.current.type == EventType.DragPerform)
					{
						Undo.RecordObject(palette, "drop objects");

						if (isReplace && !dragFromAssets)
						{
							int sourceIndex = palette.objects.IndexOf(DragAndDrop.objectReferences[0]);

							int insertIndex = palette.objects.IndexOf(obj);
							insertIndex++;
							palette.objects.Insert(insertIndex, DragAndDrop.objectReferences[0]);

							if (sourceIndex > insertIndex)
								sourceIndex++;

							palette.objects.RemoveAt(sourceIndex);
						}

						if(dragFromAssets)
						{ 
							int insertIndex = palette.objects.IndexOf(obj);
							insertIndex++;
							foreach (var dragObj in DragAndDrop.objectReferences)
							{
								if (!palette.objects.Contains(dragObj))
								{
									palette.objects.Insert(insertIndex, dragObj);
									insertIndex++;
								}
							}
						}

						EditorUtility.SetDirty(palette);
						Event.current.Use();

						DragAndDrop.AcceptDrag();
						DragAndDrop.PrepareStartDrag();
					}
				}
			}

			objectToTexture.TryGetValue(obj, out var texture);
			if (texture)
			{
				GUI.DrawTexture(rect, objectToTexture[obj]);
			}
			else
			{
				objectToTexture[obj] = AssetPreview.GetAssetPreview(obj);
				GUI.Box(rect, obj.name);
			}

			return isClick;
		}


		void Reload()
		{
			palettes = null;
			ResetTextures();
			GetPalettes();

			selectedPalette = SavedSelectedPalette;
			palettesNames = palettes
				.Select(p => p.name)
				.ToArray();

			scenesPalette = palettes.FirstOrDefault(p => p.name == "Scenes");
		}

		void GetPalettes()
		{
			palettes = LoadFromResource();

			if (palettes.Length == 0)
			{
				ObjectsPalette asset = CreateInstance<ObjectsPalette>();
				DirectoryInfo dir = new DirectoryInfo($"Assets/Editor/Resources/{assetPath}");
				if (!dir.Exists)
					dir.Create();

				AssetDatabase.CreateAsset(asset, $"Assets/Editor/Resources/{assetPath}/DefaultPalette.asset");
				AssetDatabase.SaveAssets();

				palettes = LoadFromResource();
			}
		}
 
		ObjectsPalette[] LoadFromResource()
		{
			return Resources
				.LoadAll<ObjectsPalette>(assetPath)
				.Where(x=>!x.hideInPaletteWindow)
				.ToArray();
		}
	}
}