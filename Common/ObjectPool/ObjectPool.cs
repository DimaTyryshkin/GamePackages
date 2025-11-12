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

        //readonly HashSet<GameObject> instances = new HashSet<GameObject>();

        public ObjectPool(GameObject prefab, Transform root, int initialSize = 1)
        {
            Assert.IsNotNull(prefab);
            Assert.IsNotNull(root);
            this.prefab = prefab;
            this.root = root;

            for (var i = 0; i < initialSize; i++)
            {
                var obj = CreateInstance();
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public bool IsPoolForPrefab(GameObject prefab)
        {
            return this.prefab == prefab;
        }

        public GameObject GetObject()
        {
            var obj = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
            obj.SetActive(true);
            return obj;
        }

        public void ReturnObject(GameObject go)
        {
            Assert.IsNotNull(go);
            //Assert.IsTrue(IsObjectFromPool(go));

            go.SetActive(false);
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
            //instances.Add(obj);
            obj.transform.SetParent(root);
            return obj;
        }
    }
}

