using System.Collections.Generic;
using System.Linq;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.ForestGenerator
{
	[AddComponentMenu("GamePackages/ForestGenerator/PathShape")]
	public class PathShape : PathShapeBase
	{   
#if UNITY_EDITOR
		[SerializeField, HideInInspector, IsntNull] Transform[] pathPoints;

		protected override Vector3[] PathPoints 
		{
			get
			{
				return pathPoints.Select(x=>x.transform.position).ToArray();
			}
		} 

	 
		[Button()]
		public override void FindPoints()
		{ 
			Undo.RecordObject(this, "Load"); 

			List<Transform> points = new List<Transform>();
			points.AddRange(GetComponentsInChildren<Transform>());
			points.RemoveAt(0);
			
			pathPoints = points.ToArray();
		} 
#endif
	}
}