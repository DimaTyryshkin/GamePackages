using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace GamePackages.ForestGenerator
{
	[AddComponentMenu("GamePackages/ForestGenerator/ForestDrawer")]
	public class ForestDrawer : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField, IsntNull] ForestBuilder forest;
		[SerializeField] float ageScale;

		IReadOnlyList<ForestPointInfo> forestPoints;
 
		// [Button()]
		// void Build()
		// {
		// 	forest.Init(); 
		// 	forestPoints = forest.Points; 
		// }
		
		// [Button()]
		// void Clear()
		// {
		// 	forest.Init();
		// 	forestPoints = null;
		// }

		void Update()
		{
		}

		void OnDrawGizmos()
		{
			if(!enabled)
				return;
			
			var allElements = GetComponentsInChildren<ForestShape>();
			foreach (ForestShape element in allElements)
			{
				if(element.enabled)
					element.DrawGizmos();
			}
			 
			Gizmos.color = Color.white;
			GizmosExtension.DrawBounds(forest.Bounds);
 
			if (forestPoints != null  && forestPoints.Count > 0)
			{
				float maxAge = forestPoints.Max(x => x.age);
				
				for (int i = 0; i < forestPoints.Count; i++)
				{
					if (forestPoints[i].age > 0)
					{
						float t = forestPoints[i].age / maxAge;
						Gizmos.color = Color.Lerp(Color.white, Color.black, t);
						Vector3 p1 = forestPoints[i].point;
						Vector3 p2 = p1 + Vector3.up * forestPoints[i].age * ageScale;
						Gizmos.DrawLine(p1, p2);
					}
				}
			} 
		}
#endif
	}
}