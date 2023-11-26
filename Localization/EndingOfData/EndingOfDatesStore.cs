using System;
using System.Globalization;

namespace GamePackages.Localization.EndingOfNumerals
{
	public static class EndingOfDatesStore
	{
		static EndingOfDates endingOfDatesFullCach;
		static EndingOfDates endingOfDatesShortCach;
			  
		internal static void Reset()
		{
			endingOfDatesFullCach = null;
			endingOfDatesShortCach = null;
		}


		public static EndingOfDates GetFull()
		{
			if (endingOfDatesFullCach == null)
				endingOfDatesFullCach = CreateFull(LocalizationSettings.CultureInfo);

			return endingOfDatesFullCach;
		}
		
		public static EndingOfDates GetShort()
		{
			if (endingOfDatesShortCach == null)
				endingOfDatesShortCach = CreateShort(LocalizationSettings.CultureInfo);

			return endingOfDatesShortCach;
		}

		public static EndingOfDates CreateFull(CultureInfo cultureInfo)
		{
			return new EndingOfDates(GetBuilder(cultureInfo).BuildFull());
		}

		public static EndingOfDates CreateShort(CultureInfo cultureInfo)
		{
			return new EndingOfDates(GetBuilder(cultureInfo).BuildShort());
		}

		static EndingOfDatesBuilder GetBuilder(CultureInfo cultureInfo)
		{
			if (cultureInfo.Equals(LocalizationSettings.En))
				return new EnEndingOfDatesBuilder();
			if (cultureInfo.Equals(LocalizationSettings.Ru))
				return new RuEndingOfDatesBuilder();

			throw new NotSupportedException($"cultureInfo '{cultureInfo}' not supported ");
		}
	}
}