using UnityEngine;

namespace GamePackages.Core
{
	public class PooledObject : MonoBehaviour
	{
		public ObjectPool pool;
		
		public void ReturnToPool()
		{
			pool.ReturnObject(this);
		}

		public void ReturnToPool(float time)
		{
			pool.ReturnObject(this, time);
		}
	}
}