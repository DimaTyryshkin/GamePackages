using UnityEngine;

namespace GamePackages.Core
{
	public static class RoundingUtil
	{
		public static string SecondTime(float seconds)
		{
			if (seconds < 60)
			{
				return SecondRoundWithStep(seconds, 10);
			}

			if (seconds < 2 * 60)
			{
				return SecondRoundWithStep(seconds, 20);
			}

			if (seconds < 10 * 60)
			{
				return SecondRoundToMitute(seconds);
			}

			int minutes = Mathf.RoundToInt(seconds / 60f);
			return RoundLongValue(minutes) + "_min";
		}

		public static string SecondRoundToMitute(float second)
		{
			return (Mathf.RoundToInt(second / 60f)).ToString() + "_min";
		}

		public static string SecondRoundTo5Mitute(float second)
		{
			return (Mathf.RoundToInt(second / (60f * 5f)) * 5).ToString() + "_min";
		}
 
		public static string SecondRoundWithStep(float second, int step)
		{
			return RoundWithStep(second, step) + "_sec";
		}

		/// <summary>
		/// Оставляет одну значащую фифру с округлением
		/// </summary> 
		public static int RoundLongValue(int longValue)
		{
			int divider = 1; //порядок числа

			while (divider > 0)
			{
				float r = longValue / (float) divider;

				if (Mathf.Abs(r) >= 10)
				{
					divider *= 10;
				}
				else
				{
					break;
				}
			}

			return RoundWithStep(longValue, divider);
		}

		/// <summary>
		/// Округление с произвольным шагом
		/// </summary>
		public static int RoundWithStep(float valueOrigin, int step)
		{
			return Mathf.RoundToInt(valueOrigin / step) * step;
		}

	}
}