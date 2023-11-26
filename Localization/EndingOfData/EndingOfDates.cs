using UnityEngine.Assertions;

namespace GamePackages.Localization.EndingOfNumerals
{
	public class EndingOfDates
	{
		readonly EndingOfDatesDataSet endingOfDates;

		internal EndingOfDates()
		{
			
		}

		internal EndingOfDates(EndingOfDatesDataSet endingOfDates)
		{
			Assert.IsNotNull(endingOfDates);
			this.endingOfDates = endingOfDates;
		}

		public string Days(int    n) => endingOfDates.days.Get(n);
		public string Hours(int   n) => endingOfDates.hours.Get(n);
		public string Minutes(int n) => endingOfDates.minutes.Get(n);
		public string Seconds(int n) => endingOfDates.seconds.Get(n);
	}
}