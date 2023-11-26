using GamePackages.Core;
using UnityEngine;

namespace GamePackages.ForestGenerator
{
	public abstract class PathShapeBase : ForestShape
	{ 
#if UNITY_EDITOR
		public float range;
		public float fade;
 
		protected abstract Vector3[] PathPoints { get; }

		void OnValidate()
		{
			fade = Mathf.Max(0, fade);
		}
 
		public override void DrawGizmos()
		{  
			Vector3 p1 = PathPoints[0];
			for (int i = 1; i < PathPoints.Length; i++)
			{
				Vector3 p2 = PathPoints[i];
				//Gizmos.DrawLine(p1, p2);

				Vector3 dir = (p2 - p1).normalized;
				Vector3 tangent = Vector3.Cross(Vector3.up, dir);
				Gizmos.color = GetColor1();
				Gizmos.DrawLine(p1 + tangent * range, p2 + tangent * range);
				Gizmos.DrawLine(p1 - tangent * range, p2 - tangent * range);

				Gizmos.color = GetColor2();
				Gizmos.DrawLine(p1 + tangent * (range + fade), p2 + tangent * (range + fade));
				Gizmos.DrawLine(p1 - tangent * (range + fade), p2 - tangent * (range + fade));

				p1 = p2;
			} 
		}

		public override float CorrectForestAge(Vector3 point, float originAge)
		{
			(float  minDistance, int _, Vector3 _) = MathExtension.NearPointOnSegments(PathPoints, point);
			float t = MathExtension.ArcLerp(range, range + fade, minDistance);
			return Mathf.Lerp(minAge, originAge, t);
		}

		public override float GetDistance(Vector3 point)
		{
			(float  distance, int _, Vector3 _) = MathExtension.NearPointOnSegments(PathPoints, point);
			distance = Mathf.Clamp(distance - range, 0, float.MaxValue);
			return distance;
		}
 
		public override Bounds GetBounds()
		{
			Bounds bounds = BoundsExtension.Encompass(PathPoints);
			bounds = BoundsExtension.Inflate(bounds, range + fade);
			bounds = BoundsExtension.ToFlatXZ(bounds);

			return bounds;
		} 
#endif
	}
}