using UnityEngine.Assertions;

namespace GamePackages.Localization.EndingOfNumerals
{
	internal class EndingOfDatesDataSet
	{
		public readonly IEndingOfNumerals days;
		public readonly IEndingOfNumerals hours;
		public readonly IEndingOfNumerals minutes;
		public readonly IEndingOfNumerals seconds;

		public EndingOfDatesDataSet(IEndingOfNumerals days, IEndingOfNumerals hours, IEndingOfNumerals minutes, IEndingOfNumerals seconds)
		{ 
			Assert.IsNotNull(days);
			Assert.IsNotNull(hours);
			Assert.IsNotNull(minutes);
			Assert.IsNotNull(seconds);
			
			this.days    = days;
			this.hours   = hours;
			this.minutes = minutes;
			this.seconds = seconds;
		}
	}
}