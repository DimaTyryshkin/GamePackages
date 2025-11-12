using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Core
{
    public class DynamicObjectPool : MonoBehaviour
    {
        Dictionary<GameObject, ObjectPool> prefabToPool = new Dictionary<GameObject, ObjectPool>();
        Dictionary<GameObject, ObjectPool> instancesToPool = new Dictionary<GameObject, ObjectPool>();

        public T GetInstance<T>(T prefab) where T : Component
        {
            GameObject go = Get(prefab.gameObject);
            return go.GetComponent<T>();
        }

        public GameObject Get(GameObject prefab)
        {
            Assert.IsNotNull(prefab);

            GameObject go = null;
            if (prefabToPool.TryGetValue(prefab, out ObjectPool pool))
            {
                go = pool.GetObject();
                instancesToPool[go] = pool;
            }
            else
            {
#if UNITY_EDITOR
                Assert.IsFalse(string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(prefab)), $"Объект '{prefab.name}' должен быть ассетом");
#endif

                GameObject poolGo = new GameObject(prefab.name + "_ObjectPool");
                poolGo.transform.SetParent(transform);
                ObjectPool newPool = new ObjectPool(prefab, poolGo.transform);
                prefabToPool.Add(prefab, newPool);
                go = newPool.GetObject();
                instancesToPool[go] = newPool;
            }

            return go;
        }

        public void Return(GameObject go)
        {
            if (instancesToPool.TryGetValue(go, out ObjectPool pool))
            {
                pool.ReturnObject(go);
            }
            else
            {
                throw new System.Exception($"Pool not found for go '{go.FullName()}'");
            }
        }

        public void Reset()
        {
            transform.DestroyChildren();
            prefabToPool = new Dictionary<GameObject, ObjectPool>();
            instancesToPool = new Dictionary<GameObject, ObjectPool>();
        }
    }
}