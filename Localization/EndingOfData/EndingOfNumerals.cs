namespace GamePackages.Localization.EndingOfNumerals
{
	public interface IEndingOfNumerals
	{
		string Get(int n);
	}

	internal class EndingOfNumerals : IEndingOfNumerals
	{
		string a;
		 
		private EndingOfNumerals()
		{
		}
 
		public EndingOfNumerals(string a)
		{
			this.a = a;
		}

		public string Get(int n)
		{
			return a;
		}
	}
	
	internal class EnEndingOfNumerals : IEndingOfNumerals
	{
		string a = "day";  // 1     one (day  hour  man)
		string b = "days"; // 2,32 many (days hours humans)

		private EnEndingOfNumerals()
		{
		}

		/// <summary>
		/// Порядок параметрво: day,days (1,22)
		/// </summary>
		/// <param name="a">1 day</param>
		/// <param name="b">22 days</param> 
		public EnEndingOfNumerals(string a, string b)
		{
			this.a = a;
			this.b = b;
		}

		public string Get(int n)
		{
			if (n == 1)
				return a;
			else
				return b;
		}
	}

	internal class RuEndingOfNumerals : IEndingOfNumerals
	{
		string a = "день"; // 1    один кто?       (день час   человек)
		string b = "дня";  // 2,32 ни одного кого? (дня  часа  человека)
		string c = "дней"; // 5,67 много чего?     (дней часов человек)

		private RuEndingOfNumerals()
		{
		}

		/// <summary>
		/// Порядок параметрво: день,дня,дней (1,22,57)
		/// </summary>
		/// <param name="a">1  день</param>
		/// <param name="b">22 дня</param>
		/// <param name="c">57 дней</param>
		public RuEndingOfNumerals(string a, string b, string c)
		{
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public string Get(int n)
		{
			//0 дней
			//1 день
			//2 дня
			//3 дня
			//4 дня
			//5 дней
			//6 дней
			//7 дней
			//8 дней
			//9 дней

			//11 дней
			//12 дней
			//13 дней
			//14 дней
			//15 дней
			//16 дней
			//17 дней
			//18 дней
			//19 дней 

			//Смотрим на какие цифры заканчивается число и возвращаем соответствующий вариант

			//Если кончается на 11,12...19
			int last10 = n % 100;
			if (last10 > 10 && last10 < 20)
				return c;

			//Если кончается на 0,1...9
			int last = n % 10;

			if (last > 4) //5,6,7,8,9
				return c;

			if (last > 1) //2,3,4
				return b;

			if (last == 1) //1
				return a;

			return c; //0
		}
	}
}