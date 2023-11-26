using UnityEngine.UI;

namespace GamePackages.Core
{
	public static class GraphicExtension
	{
		public static void SetAlpha(this Graphic g, float alpha)
		{
			var color = g.color;
			color.a = alpha;
			g.color = color;
		}
		
		public static void SetAlpha(this Image g, float alpha)
		{
			var color = g.color;
			color.a = alpha;
			g.color = color;
		}
	}
}