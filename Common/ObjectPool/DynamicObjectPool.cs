using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using NaughtyAttributes;







#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Core
{
    public class DynamicObjectPool : MonoBehaviour
    {
        [NonSerialized] Dictionary<GameObject, ObjectPool> prefabToPool = new Dictionary<GameObject, ObjectPool>();
        [NonSerialized] Dictionary<GameObject, ObjectPool> instancesToPool = new Dictionary<GameObject, ObjectPool>();
        [NonSerialized] List<(GameObject, ObjectPool)> returnList = new();

        [SerializeField] bool forceNewInstant;

        static DynamicObjectPool inst;
        public static DynamicObjectPool GetInst()
        {
            if (!inst)
            {
                GameObject go = new GameObject("DynamicObjectPool");
                go.hideFlags = HideFlags.DontSave;
                inst = go.AddComponent<DynamicObjectPool>();
                go.SetActive(false);
            }

            return inst;
        }

        public T Get<T>(T prefab) where T : Component
        {
            GameObject go = Get(prefab.gameObject);
            T c = go.GetComponent<T>();
            Assert.IsNotNull(c);
            //if (!c) throw new System.Exception($"Component '{nameof(T)}' not found on prefab '{prefab.gameObject.FullName()}'");
            return c;
        }


        public GameObject Get(GameObject prefab, Transform parent = null)
        {
            Assert.IsNotNull(prefab);
            if (!prefabToPool.TryGetValue(prefab, out ObjectPool pool))
            {
#if UNITY_EDITOR
                Assert.IsFalse(string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(prefab)), $"Объект '{prefab.name}' должен быть ассетом");
#endif
                GameObject poolGo = new GameObject(prefab.name);
                poolGo.transform.SetParent(transform);
                pool = new ObjectPool(prefab, poolGo.transform);
                prefabToPool.Add(prefab, pool);
            }

            GameObject go = pool.GetObject(parent);
            instancesToPool[go] = pool;
            return go;
        }

        public void Return(GameObject go)
        {
            if (instancesToPool.TryGetValue(go, out ObjectPool pool))
                Return(go, pool);
            else
                throw new System.Exception($"Pool not found for go '{go.FullName()}'");
        }

        [Button]
        public void ReturnAll()
        {
            foreach ((GameObject goToReturn, ObjectPool pool) in instancesToPool)
                pool.ReturnObject(goToReturn);

            instancesToPool.Clear();
        }

        public void ReturnChildren(GameObject root, int maxDepth = 1)
        {
            returnList.Clear();
            ReturnChildren(root, 0, maxDepth, returnList);

            foreach ((GameObject goToReturn, ObjectPool pool) in returnList)
                Return(goToReturn, pool);
        }

        void ReturnChildren(GameObject go, int depth, int maxDepth, List<(GameObject, ObjectPool)> list)
        {
            if (instancesToPool.TryGetValue(go, out ObjectPool pool))
            {
                list.Add((go, pool));
            }
            else
            {
                depth++;
                if (depth > maxDepth)
                    return;

                int count = go.transform.childCount;
                for (int n = 0; n < count; n++)
                    ReturnChildren(transform.GetChild(n).gameObject, depth, maxDepth, list);
            }
        }

        void Return(GameObject go, ObjectPool pool)
        {
            instancesToPool.Remove(go);
            pool.ReturnObject(go);
        }

        [Button]
        void SwitchForceNewInstant()
        {
            forceNewInstant = !forceNewInstant;

            foreach ((_, ObjectPool pool) in prefabToPool)
                pool.forceNewInstant = forceNewInstant;
        }
    }
}