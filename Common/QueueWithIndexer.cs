using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
	public class QueueWithIndexer<T>: IEnumerable<T>
	{
		T[] arr;

		int startIndex;
		int version;
		int length;
		bool isEnumerated;

		public int Count => length;
		public int Capacity => arr.Length;

		public T this[int i]
		{
			get => arr[QueueIndexToArrIndex(i)];
			set => arr[QueueIndexToArrIndex(i)] = value;
		}

		public QueueWithIndexer(int capacity = 0)
		{
			arr = new T[capacity];
		}

		public void Queue(T value)
		{
			if (length == arr.Length)
				Resize();
			
			int writeIndex = (startIndex + length) % arr.Length;
			arr[writeIndex] = value;

			length++;
			version++;
		}

		public T Enqueue()
		{
			Assert.IsTrue(length > 0);
            
			T value = arr[startIndex];
			startIndex = (startIndex + 1) % arr.Length;
			length--;
			version++;
			return value;
		}

		public void Clear()
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = default;
			}

			startIndex = 0;
			length = 0;
			version++;
		}


		int QueueIndexToArrIndex(int queueIndex)
		{
			if (queueIndex >= length)
				throw new IndexOutOfRangeException();
			
			if (queueIndex < 0)
				throw new IndexOutOfRangeException();
            
			return (startIndex + queueIndex) % arr.Length;
		}

		void Resize()
		{
			T[] newArr = new T[Math.Max(4, arr.Length * 2)];

			for (int i = 0; i < length; i++)
				newArr[i] = this[i];

			startIndex = 0;
			arr = newArr;
		}

		public IEnumerator<T> GetEnumerator()
		{
			long oldVersion = version;
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				if (version != oldVersion)
					throw new InvalidOperationException("Cant modify collection while it enumerated");

				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		} 
	}
}