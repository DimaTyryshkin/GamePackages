using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
	public class Pool<T>  where T : MonoBehaviour
	{
		readonly T prefab;
		readonly Transform instanceRoot;
		readonly Queue<T> pool;

		public Pool(Transform instanceRoot, T prefab)
		{
			Assert.IsNotNull(prefab);
			Assert.IsNotNull(instanceRoot);
            
			this.prefab = prefab;
			this.instanceRoot = instanceRoot;
            
			pool = new Queue<T>(4);
		}

		public void FillPool(int count)
		{
			for (int i = 0; i < count; i++)
			{
				T newInst = instanceRoot.InstantiateAsChild(prefab);
				newInst.gameObject.SetActive(false);
				newInst.transform.localScale = prefab.transform.localScale;
				pool.Enqueue(newInst);
			}
		}

		public T Get()
		{
			if (pool.Count == 0)
			{
				T newInst = instanceRoot.InstantiateAsChild(prefab);
				newInst.gameObject.SetActive(true);
				newInst.transform.localScale = prefab.transform.localScale;
				return newInst;
			}
			else
			{
				T inst = pool.Dequeue();
				inst.gameObject.SetActive(true);
				return inst;
			}
		}

		public void ReturnToPool(T inst)
		{
			Assert.IsFalse(pool.Contains(inst));
			
			inst.gameObject.SetActive(false);
			pool.Enqueue(inst);
		}
	}
}