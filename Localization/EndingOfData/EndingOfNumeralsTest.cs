using UnityEngine.Assertions;

namespace GamePackages.Localization.EndingOfNumerals
{
	public static class EndingOfNumeralsTest
	{
		readonly static int[] ruA = new[] {1, 21, 31, 101, 131,1001, 1131};
		readonly static int[] ruB = new[] {2, 3, 4, 22, 23, 24, 72, 73, 74, 122, 123, 124, 1022, 1023, 1024};
		readonly static int[] ruC = new[]
		{
			0, 5, 6, 7, 8, 9, 
			10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
			25, 26, 27, 28, 29, 30,
			105,106,107,108,109,110,111,
			160,
			1000, 1005, 1006, 1007, 1008, 1009,
			1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020,
			1055, 1056, 1057, 1058, 1059, 1060
		};
		
		
		readonly static int[] enA = new[] {1};
		readonly static int[] enB = new[] {
			2, 3, 4, 5,6,7,8,9,
			10,12,13,14,15,16,17,18,19,20,
			21,22,23,24,
			71,72, 73,
			100,101, 121, 124,
			1000,1001, 1010
		};

		
		public static void RuEndingOfNumerals()
		{
			RuEndingOfNumerals r= new RuEndingOfNumerals("день","дня","дней");
			foreach (int n in ruA) 
				Assert.AreEqual("день", r.Get(n));  
			
			foreach (int n in ruB) 
				Assert.AreEqual("дня", r.Get(n));  
			
			foreach (int n in ruC) 
				Assert.AreEqual("дней", r.Get(n)); 
		}
		
		public static void EnEndingOfNumerals()
		{
			EnEndingOfNumerals r = new EnEndingOfNumerals("day","days");
			foreach (int n in enA) 
				Assert.AreEqual("day", r.Get(n));  
			
			foreach (int n in enB) 
				Assert.AreEqual("days", r.Get(n));  
		}

		public static void RuEndingOfDatesBuilder()
		{
			EndingOfDates r = EndingOfDatesStore.CreateFull(LocalizationSettings.Ru);
			
			foreach (int n in ruA)
			{
				Assert.AreEqual("День", r.Days(n));
				Assert.AreEqual("Час", r.Hours(n));
				Assert.AreEqual("Минута", r.Minutes(n));
				Assert.AreEqual("Секунда", r.Seconds(n));
			}
			
			foreach (int n in ruB)
			{
				Assert.AreEqual("Дня", r.Days(n));
				Assert.AreEqual("Часа", r.Hours(n));
				Assert.AreEqual("Минуты", r.Minutes(n));
				Assert.AreEqual("Секунды", r.Seconds(n));
			}
			
			foreach (int n in ruC)
			{
				Assert.AreEqual("Дней", r.Days(n));
				Assert.AreEqual("Часов", r.Hours(n));
				Assert.AreEqual("Минут", r.Minutes(n));
				Assert.AreEqual("Секунд", r.Seconds(n));
			}  
		}

		public static void EnEndingOfDatesBuilder()
		{
			EndingOfDates r = EndingOfDatesStore.CreateFull(LocalizationSettings.En);
			
			foreach (int n in enA)
			{
				Assert.AreEqual("Day", r.Days(n));
				Assert.AreEqual("Hour", r.Hours(n));
				Assert.AreEqual("Minute", r.Minutes(n));
				Assert.AreEqual("Second", r.Seconds(n));
			}
			
			foreach (int n in enB)
			{
				Assert.AreEqual("Days", r.Days(n));
				Assert.AreEqual("Hours", r.Hours(n));
				Assert.AreEqual("Minutes", r.Minutes(n));
				Assert.AreEqual("Seconds", r.Seconds(n));
			}
		}
	}
}