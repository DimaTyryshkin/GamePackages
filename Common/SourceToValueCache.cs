using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{ 
	public abstract class SourceToValueCache
	{
		protected static readonly Dictionary<Type, SourceToValueCache> cacheOfCache = new Dictionary<Type, SourceToValueCache>();

		protected abstract void ClearInternal();
		
		public static void Clear()
		{
			foreach (KeyValuePair<Type, SourceToValueCache> va in cacheOfCache)
				va.Value.ClearInternal();
		}
	}

	public class SourceToValueCache<TSource, TValue>:SourceToValueCache
	{
		readonly Dictionary<TSource, TValue> sourceToValueCache = new Dictionary<TSource, TValue>();
		readonly Func<TSource, TValue> getValueFunc;


		public static SourceToValueCache<TSource, TValue> Get(Func<TSource, TValue> getValueFunc)
		{
			object cache = cacheOfCache.GetOrCreate(typeof(TValue), () => new SourceToValueCache<TSource, TValue>(getValueFunc));
			return (SourceToValueCache<TSource, TValue>)cache;
		}
 
		SourceToValueCache(Func<TSource, TValue> getValueFunc)
		{
			Assert.IsNotNull(getValueFunc);
			this.getValueFunc = getValueFunc;
		}

		public TValue Get(TSource source)
		{
			if (sourceToValueCache.TryGetValue(source, out TValue aiCarT))
			{
				return aiCarT;
			}
			else
			{
				TValue value = getValueFunc(source);

				sourceToValueCache.Add(source, value);
				return value;
			}
		}
 
		protected override void ClearInternal()
		{
			sourceToValueCache.Clear();
		}
	}
}