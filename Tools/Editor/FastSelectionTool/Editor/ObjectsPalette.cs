using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Tools.ObjectsPalette
{
	public class ObjectsPalette : ScriptableObject
	{
		public bool isLevelDesignPalette;
		public bool hideInPaletteWindow;
		public List<Object> objects = new List<Object>();
		public List<ObjectsPalette> subPalettes = new List<ObjectsPalette>();

		public void Remove(Object obj)
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Remove");
			EditorUtility.SetDirty(this);
#endif
			objects.Remove(obj);

			//foreach (var subPalette in subPalettes)
			//	subPalette.Remove(obj);
		}

		[Button()]
		void RemoveColliders()
		{
			foreach (var obj in objects)
			{
				var go = obj as GameObject;
				if(go)
				{
					var allColliders = go.GetComponentsInChildren<Collider>();
					foreach (var collider in allColliders)
					{
						EditorUtility.SetDirty(go);
						EditorUtility.SetDirty(collider.gameObject);
						Undo.DestroyObjectImmediate(collider);
					}
				}
			}
		}
	}
}