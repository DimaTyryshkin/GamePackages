using GamePackages.Core;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.ForestGenerator
{
	[AddComponentMenu("GamePackages/ForestGenerator/CircleShape")]
	public class CircleShape : ForestShape
	{
#if UNITY_EDITOR
		static readonly Quaternion Rotation = Quaternion.LookRotation(Vector3.up);
		
		public float range;
		public float fade;
		 
		void OnValidate()
		{
			fade = Mathf.Max(0, fade);
		}
 
		public override float CorrectForestAge(Vector3 point, float originAge)
		{
			float distance = Vector3.Distance(transform.position, point);
			float t = MathExtension.ArcLerp(range, range + fade, distance);
			return Mathf.Lerp(minAge, originAge, t);
		}

		public override float GetDistance(Vector3 point)
		{
			float distance = Vector3.Distance(transform.position, point);
			distance = Mathf.Clamp(distance - range, 0, float.MaxValue);
			return distance;
		}

		public override void DrawGizmos()
		{
			Handles.color = GetColor1();
			Handles.CircleHandleCap(0, transform.position, Rotation, range, EventType.Repaint);
			
			Handles.color = GetColor2();
			Handles.CircleHandleCap(0, transform.position, Rotation, range + fade, EventType.Repaint);
			
		}
		
		public override Bounds GetBounds()
		{
			Bounds bounds = new Bounds(transform.position, Vector3.zero);
			bounds = BoundsExtension.Inflate(bounds, range + fade);
			bounds = BoundsExtension.ToFlatXZ(bounds); 

			return bounds;
		}

		public override void FindPoints()
		{
			// ничего не надо делать
		}
#endif
	}
}