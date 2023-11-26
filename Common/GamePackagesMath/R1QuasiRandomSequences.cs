using UnityEngine;

namespace GamePackages.GamePackagesMath
{
	/// <summary>
	/// see https://habr.com/ru/post/440892/ or https://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/#content
	/// </summary>
	public class R1QuasiRandomSequences
	{
		// One dimension
		static double g1 = 1.6180339887498948482d;
		static double a11 = 1.0d / g1;

		// Two dimensions
		static double g2 = 1.32471795724474602596d;
		static double a21 = 1.0d / g2;
		static double a22 = 1.0d / (g2 * g2);

		// Three dimensions
		static double g3 = 1.22074408460575947536d;
		static double a31 = 1.0d / g3;
		static double a32 = 1.0d / (g3 * g3);
		static double a33 = 1.0d / (g3 * g3 * g3);

		readonly double offset;

		public R1QuasiRandomSequences()
		{
			offset = 0.5f;
		}
		
		public R1QuasiRandomSequences(double offset)
		{
			this.offset = offset;
		}

		/// <summary>
		/// One dimension
		/// </summary>
		public double GetD1(int n)
		{
			return (offset + a11 * n) % 1;
		}

		public float GetD1Float(int n)
		{
			return (float)GetD1(n);
		}

		/// <summary>
		/// Two dimensions
		/// </summary>
		public (double, double) GetD2(int n)
		{
			double r1 = (offset + a21 * n) % 1;
			double r2 = (offset + a22 * n) % 1;
			return (r1, r2);
		}

		public Vector2 GetD2Float(int n)
		{
			(double r1, double r2) = GetD2(n);
			return new Vector2((float)r1, (float)r2);
		}


		/// <summary>
		/// Three dimension
		/// </summary>
		public (double, double, double) GetD3(int n)
		{
			double r1 = (offset + a31 * n) % 1;
			double r2 = (offset + a32 * n) % 1;
			double r3 = (offset + a33 * n) % 1;
			return (r1, r2, r3);
		}

		public Vector3 GetD3Float(int n)
		{
			(double r1, double r2, double r3) = GetD3(n);
			return new Vector3((float)r1, (float)r2, (float)r3);
		}
	}
}