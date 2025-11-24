using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    internal class ObjectPool
    {
        private GameObject prefab;
        private Transform root;
        readonly Queue<GameObject> pool = new Queue<GameObject>();

        public bool forceNewInstant;

        public ObjectPool(GameObject prefab, Transform root, int initialSize = 0)
        {
            Assert.IsNotNull(prefab);
            Assert.IsNotNull(root);
            this.prefab = prefab;
            this.root = root;
        }

        public bool IsPoolForPrefab(GameObject prefab)
        {
            return this.prefab == prefab;
        }

        public GameObject GetObject(Transform parent)
        {
            GameObject obj = (pool.Count > 0 && !forceNewInstant) ?
                pool.Dequeue() :
                CreateInstance();

            obj.transform.SetParent(parent);
            return obj;
        }

        public void ReturnObject(GameObject go)
        {
            go.transform.SetParent(root);
            pool.Enqueue(go);
        }

        //public bool IsObjectFromPool(GameObject go) => instances.Contains(go);

        //public void Reset()
        //{
        //    foreach (var instance in instances)
        //        ReturnObject(instance);
        //}

        /// <summary>
        /// Creates a new instance of the pooled object type.
        /// </summary>
        /// <returns>A new instance of the pooled object type.</returns>
        GameObject CreateInstance()
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(root);
            return obj;
        }
    }
}

