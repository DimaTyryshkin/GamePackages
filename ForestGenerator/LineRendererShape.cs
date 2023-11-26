using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace GamePackages.ForestGenerator
{
	[AddComponentMenu("GamePackages/ForestGenerator/LineRendererShape")]
	public class LineRendererShape : PathShapeBase
	{
#if UNITY_EDITOR
		[SerializeField, IsntNull] LineRenderer lineRenderer;
		[SerializeField, HideInInspector] Vector3[] pathPoints;

		protected override Vector3[] PathPoints => pathPoints;

		[Button()]
		public override void FindPoints()
		{
			pathPoints = new Vector3[lineRenderer.positionCount];
			for (int i = 0; i < lineRenderer.positionCount; i++)
				pathPoints[i] = lineRenderer.GetPosition(i); 
		}
#endif
	}
}