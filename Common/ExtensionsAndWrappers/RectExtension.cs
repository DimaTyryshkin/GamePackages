using System.Collections.Generic;
using UnityEngine;

namespace GamePackages.Core
{
	public static class RectExtension
	{ 
		public static Rect ScaleSize(this Rect rect, float scale)
		{
			return rect.ScaleSize(scale, rect.center);
		}
 
		public static Rect ScaleSize(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;

			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;

			result.x += pivotPoint.x;
			result.y += pivotPoint.y;

			return result;
		}
		
		public static Rect SplitGuiRectOnLines(this Rect rect, int linesAmount, int lineIndex)
		{
			if (linesAmount == 1)
				return rect;
			
			Vector2 pos = rect.position;
			Vector2 size = rect.size;

			float lineHeight = size.y / linesAmount;

			return new Rect(pos.x, pos.y + lineHeight * lineIndex, size.x, lineHeight);
		}

		public static IEnumerable<Vector2Int> ForEach(this RectInt rect)
		{
			var min = rect.min;
			var max = rect.max;
			for (int x = min.x; x < max.x; x++)
			{
				for (int y = min.y; y < max.y; y++)
				{
					yield return new Vector2Int(x, y);
				}
			}
		}
	}
}