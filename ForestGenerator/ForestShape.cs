using UnityEngine;

namespace GamePackages.ForestGenerator
{
	public abstract class ForestShape : MonoBehaviour
	{
#if UNITY_EDITOR
		public enum ShapeType
		{
			forestSource = 0,
			deForest,
			border,
		}

		protected static readonly Color forestColor1 = new Color(0, 0.4f, 0);
		protected static readonly Color forestColor2 = new Color(0, 1f, 0);
		
		protected static readonly Color deForestColor1 = new Color(0.4f, 0, 0);
		protected static readonly Color deForestColor2 = new Color(1, 0, 0);
		
		public ShapeType type;
		
		public int minAge;
		public abstract float CorrectForestAge(Vector3 point, float originAge);
		public abstract float GetDistance(Vector3 point);
		public abstract void DrawGizmos();
		public abstract Bounds GetBounds();

		protected Color GetColor1()
		{
			switch (type)
			{
				case ShapeType.forestSource:
					return forestColor1;
				
				case ShapeType.deForest:
					return deForestColor1;
				
				default:
					return Color.gray;
			}
		}

		protected Color GetColor2()
		{
			switch (type)
			{
				case ShapeType.forestSource:
					return forestColor2;
				
				case ShapeType.deForest:
					return deForestColor2;
				
				default:
					return Color.white;
			}
		}
		
		public abstract void FindPoints();

		void Update()
		{
			
		}
#endif
	}
}