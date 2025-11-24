using UnityEngine;

namespace GamePackages.Core
{
    public static class ColorExtension
    {
        public static Color Create(float rgb) => new Color(rgb, rgb, rgb, 1);
        public static Color Create(float rgb, float a) => new Color(rgb, rgb, rgb, a);

        public static Color GetMaxComponenst(Color c1, Color c2)
        {
            return new Color(
                Mathf.Max(c1.r, c2.r),
                Mathf.Max(c1.g, c2.g),
                Mathf.Max(c1.b, c2.b),
                Mathf.Max(c1.a, c2.a));
        }

        public static Color Clamp(Color value, Color min, Color max)
        {
            return new Color(
                Mathf.Clamp(value.r, min.r, max.r),
                Mathf.Clamp(value.g, min.g, max.g),
                Mathf.Clamp(value.b, min.b, max.b),
                Mathf.Clamp(value.a, min.a, max.a));
        }

        public static Color Negative(Color c)
        {
            return new Color(
                1 - c.r,
                1 - c.g,
                1 - c.g,
                c.a);
        }

        public static bool CompareRGBApproximately(Color c1, Color c2)
        {
            return
                Mathf.Approximately(c1.r, c2.r) &&
                Mathf.Approximately(c1.g, c2.g) &&
                Mathf.Approximately(c1.b, c2.b);
        }

        public static bool GreaterThen(this Color a, Color b)
        {
            return
               (a.r > b.r) &&
               (a.g > b.g) &&
               (a.b > b.b);
        }
    }
}