using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace GamePackages.Core
{
	public static class EnumerableExtension
	{
		public static void ToStringMultilineAndLog<T>(this IEnumerable<T> collection, string header = null, Func<T, string> func = null, int tabCount = 0)
		{
			string log =collection.ToStringMultiline(header, func, tabCount);
			//Debug.Log(log); 
		}

		public static string ToStringMultiline<T>(this IEnumerable<T> collection, string header = null, Func<T, string> func = null, int tabCount = 0)
		{
			if (func == null)
				return collection.ToStringMultilineWithIndex(header, (t, i) =>(t == null ? "null" : t.ToString()), tabCount);
			else
				return collection.ToStringMultilineWithIndex(header, (t, i) => func(t), tabCount);
		}

		public static string ToStringMultilineWithIndex<T>(this IEnumerable<T> collection, string header = null, Func<T,int,string> func = null, int tabCount = 0 )
		{
			if (func == null)
				func = (obj, i) => $"[{i:00}] '{obj}'"; 

			string tabs = "";
			for (int i = 0; i < tabCount; i++)
				tabs += "	"
					;
			string s = "";

			if (header != null)
				s +=$"{tabs}{header} [{collection.Count()}]{Environment.NewLine}";

			string prefix = string.IsNullOrWhiteSpace(header) ? "" : "    ";

			int index = 0;
			foreach (var element in collection)
			{
				s += tabs + prefix + func(element,index) + Environment.NewLine;
				index++;
			}

			return s;
		}

		public static IEnumerable<T> CastIfCan<TSource,T>(this IEnumerable<TSource> collection)
		{
			foreach (var element in collection)
			{
				if (element is T value)
					yield return value;
			}
		} 
	}
	
	public static class StringBuilderExtension
	{
		public static void AppendLine<T>(this StringBuilder stringBuilder, T value)
		{
			stringBuilder.AppendLine(value.ToString());
		}
		
		public static void Log(this StringBuilder stringBuilder)
		{
			Debug.Log(stringBuilder.ToString());
		}
	}

	public static class StringExtension
	{
		public static string TrimEnd(this string origin, int count)
		{
			return origin.Substring(0, origin.Length - count);
		}
		
		public static bool ContainAny(this string origin, IList<string> values)
		{
			foreach (var value in values)
			{
				if (origin.Contains(value))
					return true;
			} 
			
			return false;
		}
		 		
		public static int FindSubstringIndex(this string origin, string subString)
		{
			for (int i = 0; i < origin.Length; i++)
			{
				for (int j = 0; j < subString.Length; j++)
				{
					if (origin[i + j] != subString[j])
						break;

					if (j == subString.Length - 1)
						return i;
				}
			}

			return -1;
		}
	}

	public static class DictionaryWrapper
	{
		public static T2 GetOrDefault<T1, T2>(this IReadOnlyDictionary<T1, T2> dic, T1 key, T2 defaultValue)
		{
			if (dic.TryGetValue(key, out T2 val))
				return val;
			else
				return defaultValue;
		}
		
		public static T2 GetOrCreate<T1, T2>(this Dictionary<T1, T2> dic, T1 key, Func<T2>  factory)
		{
			if (dic.TryGetValue(key, out T2 val))
				return val;
			else
			{
				 var defaultValue =factory();
				 dic.Add(key, defaultValue);
				 return defaultValue;
			}
		}
	}

	public static class RandomExtension
	{
		public static float Next(this Random rnd, float min, float max)
		{
			return min + (float)rnd.NextDouble() * (max - min);
		}
	}

	public static class MathExtension
	{
		/// <summary>
		/// Остаток от деления. Всегда положительный
		/// </summary> 
		public static int Residual(int value, int maxValue)
		{
			int result = value % maxValue;
			if (Math.Sign(result) * Math.Sign(maxValue) < 0)
				result += maxValue;
			return result;
		}
		
		
		/// <summary>
		/// Like Sin(x) and ArcSin(x)
		/// </summary>
		public static float ArcLerp(float star, float end, float value)
		{
			float delta = end - star;

			if (Mathf.Approximately(delta, 0))
				return value < star ? 0 : 1; 

			float x = value - star;
			return Mathf.Clamp01(x / delta);
		}
		
		/// <summary>
		/// Переводит value из одного диапозона в другой.
		/// Удобно, чтобы перевести Sin(value) из диапозона [-1,1] в [toA, toB]
		/// </summary>
		public static float Map(float fromA, float fromB, float toA, float toB,float value)
		{
			float mappedValue = MapUnclamped(fromA, fromB, toA, toB, value);
			(float min, float max) = toB >= toA ? 
				(toA, toB):
				(toB, toA);

			return Mathf.Clamp(mappedValue, min, max);
		}
		
		/// <summary>
		/// Переводит value из одного диапозона в другой.
		/// Удобно, чтобы перевести Sin(value) из диапозона [-1,1] в [toA, toB]
		/// </summary>
		public static float MapUnclamped(float fromA, float fromB, float toA, float toB,float value)
		{
			float fromDelta = fromB - fromA;
			float x = Mathf.Approximately(fromDelta, 0) ? 
				(value < fromA ? 0 : 1 ):
				(value - fromA)/fromDelta;

			return toA + (toB - toA) * x;
		}

		public static (float, int, Vector3) NearPointOnSegments(Vector3[] segments, Vector3 point)
		{
			float minDistance = float.MaxValue;
			int index = 0;
			Vector3 nearPointOnSegment = segments[0];
			for (int i = 0; i < segments.Length - 1; i++)
			{
				Vector3 p1 = segments[i + 0];
				Vector3 p2 = segments[i + 1];
				Vector3 pointOnSegment = NearPointOnSegment(p1, p2, point);

				float distance = Vector3.Distance(pointOnSegment, point); 
				if ( distance <= minDistance)
				{
					minDistance = distance;
					index = i;
					nearPointOnSegment = pointOnSegment;
				}
			}

			return (minDistance, index, nearPointOnSegment);
		}

		public static Vector3 NearPointOnSegment(Vector3 p1, Vector3 p2, Vector3 point)
		{
			Vector3 segment = p2 - p1;
			float dot1 = Vector3.Dot(point - p1, segment);
			float dot2 = -Vector3.Dot(point - p2, segment);

			if (dot1 >= 0 && dot2 >= 0)
			{
				float d = GetPlaneD(segment, point);
				//Уравнение плоскости, проходящей через точку point с нормалью segment будет таким: segment*p + d == 0
				Vector3 crossPoint = CrossLineAndPlane(p1, p2, segment, d);
				return crossPoint;
			}
			else
			{
				if (dot1 > dot2)
					return p2;
				else
					return p1;
			}
		}

		public static float GetPlaneD(Vector3 planeNormal, Vector3 pointOnPlane)
		{
			return -Vector3.Dot(pointOnPlane, planeNormal);
		}

		public static Vector3 CrossLineAndPlane(Vector3 lineP1, Vector3 lineP2, Vector3 planeNormal, float planeD)
		{
			Vector3 lineDir = lineP2 - lineP1;

			float dot1 = Vector3.Dot(planeNormal, lineP1);
			float dot2 = Vector3.Dot(planeNormal, lineDir);
			float t = -(dot1 + planeD) / dot2;

			Vector3 p = lineP1 + lineDir * t;
			return p;
		} 
		
		public static int PingPong(int t, int length)
		{
			int q = t / length;
			int r = t % length;

			if ((q % 2) == 0)
				return r;
			else
				return length - r;
		}
	}
  
	public static class ListExtension
	{		
		public static T Random<T>(this IList<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static void ShiftRight<T>(this IList<T> list, int count)
		{
			if (list.Count < 2)
				return;

			int lastIndex = list.Count - 1;
			
			for (int n = 0; n < count; n++)
			{
				T tmp = list[lastIndex];
				for (int i = lastIndex; i > 0; i--)
				{
					list[i] = list[i - 1];
				}

				list[0] = tmp;
			}
		}

		public static int RandomIndex<T>(this IList<T> list)
		{
			return UnityEngine.Random.Range(0, list.Count);
		}
		
		public static T Last<T>(this IList<T> list)
		{
			return list[list.Count - 1];
		}
		 
		/// <summary>
		/// Shuffles the specified list.
		/// </summary> 
		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k     = UnityEngine.Random.Range(0, n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static List<T> GetRandomItems<T>(this IEnumerable<T> a, int amount)
		{
			var list = a.ToList();
			List<T> result = new List<T>(amount);
			Assert.IsTrue(list.Count >= amount, "list.Count >= amount");
			for (int i = 0; i < amount; i++)
			{
				int index = list.RandomIndex();
				result.Add(list[index]);
				list.RemoveAt(index);
			}

			return result;
		}
		
		public static T MinItem<T>(this IList<T> collection, Func<T,float> selector)
		{ 
			int minIndex = 0;
			float minValue =  selector(collection[0]);

			for (int i = 1; i < collection.Count; i++)
			{
				float value = selector(collection[i]);
				if (value < minValue)
				{
					minValue = value;
					minIndex = i;
				}
			}

			return collection[minIndex];
		}
		
		public static T MaxItem<T>(this IList<T> collection, Func<T,float> selector)
		{ 
			int maxIndex = 0;
			float maxValue = selector(collection[0]);

			for (int i = 1; i < collection.Count; i++)
			{
				float value = selector(collection[i]);
				if (value > maxValue)
				{
					maxValue = value;
					maxIndex = i;
				}
			}

			return collection[maxIndex];
		}

		public static int MinIndex<T>(this IList<T> collection, Func<T, float> selector)
		{
			return MinIndex(collection, selector, collection.Count);
		}

		public static int MinIndex<T>(this IList<T> collection, Func<T,float> selector, int count)
		{
			if (count == 0)
				return -1;
			
			int minIndex = 0;
			float minValue =  selector(collection[0]);

			for (int i = 1; i < count; i++)
			{
				float value = selector(collection[i]);
				if (value < minValue)
				{
					minValue = value;
					minIndex = i;
				}
			}

			return minIndex;
		}
		
		
		public static int MaxIndex<T>(this IList<T> collection, Func<T, float> selector)
		{
			return MaxIndex(collection, selector, collection.Count);
		}
		
		public static int MaxIndex<T>(this IList<T> collection, Func<T,float> selector, int count)
		{ 
			if (count == 0)
				return -1;
			
			int maxIndex = 0;
			float maxValue = selector(collection[0]);

			for (int i = 1; i < count; i++)
			{
				float value = selector(collection[i]);
				if (value > maxValue)
				{
					maxValue = value;
					maxIndex = i;
				}
			}

			return maxIndex;
		}
		
		public static void Replace<T>(this IList<T> collection, int i1, int i2)
		{
			T temp = collection[i1];
			collection[i1] = collection[i2];
			collection[i2] = temp;
		}
	}

	public static class IEnumerableExtension
	{

		public static T FirstOrDefault<T>(IEnumerable<T> collection, Predicate<T> predicate) where T : class
		{
			foreach (var element in collection)
			{
				if (predicate(element))
					return element;
			}

			return null;
		}
	}

	public static class ArrayExtension
	{
		public static T Random<T>(this T[] array)
		{
			return array[UnityEngine.Random.Range(0, array.Length)];
		}
		
		public static void Replace<T>(this T[] array, int a, int b)
		{
			var temp = array[a];
			array[a] = array[b];
			array[b] = temp;
		}

		public static T Random<T>(this T[] array, Random rnd)
		{ 
			return array[rnd.Next(0, array.Length)];
		}

		public static int RandomIndex<T>(this T[] array)
		{
			return UnityEngine.Random.Range(0, array.Length);
		}
		
		public static T Last<T>(this T[] array)
		{
			return array[array.Length - 1];
		}

		public static int IndexOf<T>(this T[] array, T element)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(element))
					return i;
			}

			return -1;
		}

		public static int IndexOf<T>(this T[] array, Predicate<T> predicate)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (predicate(array[i]))
					return i;
			}

			return -1;
		}
		
		public static void ShiftRight<T>(this  T[] list, int count)
		{
			if (list.Length < 2)
				return;

			int lastIndex = list.Length - 1;

			for (int n = 0; n < count; n++)
			{
				T tmp = list[lastIndex];
				for (int i = lastIndex; i > 0; i--)
				{
					list[i] = list[i - 1];
				}

				list[0] = tmp;
			}
		}
	}
}