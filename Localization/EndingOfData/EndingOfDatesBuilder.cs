namespace GamePackages.Localization.EndingOfNumerals
{
	internal abstract class EndingOfDatesBuilder
	{
		public abstract EndingOfDatesDataSet BuildFull();
		public abstract EndingOfDatesDataSet BuildShort();
	}
	
	internal class RuEndingOfDatesBuilder : EndingOfDatesBuilder
	{ 
		public override EndingOfDatesDataSet BuildFull()
		{
			IEndingOfNumerals days    = new RuEndingOfNumerals("День", "Дня", "Дней");
			IEndingOfNumerals hours   = new RuEndingOfNumerals("Час", "Часа", "Часов");
			IEndingOfNumerals minutes = new RuEndingOfNumerals("Минута", "Минуты", "Минут");
			IEndingOfNumerals seconds = new RuEndingOfNumerals("Секунда", "Секунды", "Секунд");
			
			return new EndingOfDatesDataSet(days, hours, minutes, seconds);
		} 
		
		public override EndingOfDatesDataSet BuildShort()
		{
			IEndingOfNumerals days    = new EndingOfNumerals("дн");
			IEndingOfNumerals hours   = new EndingOfNumerals("час");
			IEndingOfNumerals minutes = new EndingOfNumerals("мин");
			IEndingOfNumerals seconds = new EndingOfNumerals("сек");

			return new EndingOfDatesDataSet(days, hours, minutes, seconds);
		}
	}
	
	internal class EnEndingOfDatesBuilder : EndingOfDatesBuilder
	{ 
		public override EndingOfDatesDataSet BuildFull()
		{
			IEndingOfNumerals days    = new EnEndingOfNumerals("Day", "Days");
			IEndingOfNumerals hours   = new EnEndingOfNumerals("Hour", "Hours");
			IEndingOfNumerals minutes = new EnEndingOfNumerals("Minute", "Minutes");
			IEndingOfNumerals seconds = new EnEndingOfNumerals("Second", "Seconds");

			return new EndingOfDatesDataSet(days, hours, minutes, seconds);
		} 
		
		public override EndingOfDatesDataSet BuildShort()
		{
			IEndingOfNumerals days    = new EndingOfNumerals("d");
			IEndingOfNumerals hours   = new EndingOfNumerals("h");
			IEndingOfNumerals minutes = new EndingOfNumerals("min");
			IEndingOfNumerals seconds = new EndingOfNumerals("sec");

			return new EndingOfDatesDataSet(days, hours, minutes, seconds);
		}
	}
}