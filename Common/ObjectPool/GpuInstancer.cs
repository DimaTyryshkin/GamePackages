using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    public class GpuInstancer : MonoBehaviour
    {
        [NonSerialized] Dictionary<GameObject, GpuInstancerGroup> prefabToPool = new Dictionary<GameObject, GpuInstancerGroup>();

        int maxCount;

        static GpuInstancer inst;
        public static GpuInstancer GetInst(int maxCount)
        {
            if (!inst)
            {
                GameObject go = new GameObject("GpuInstancingDynamicObjectPool");
                go.hideFlags = HideFlags.DontSave;
                inst = go.AddComponent<GpuInstancer>();
                inst.maxCount = maxCount;
                go.SetActive(false);
            }

            return inst;
        }

        public void Add(GameObject prefab, Vector3 position)
        {
            Assert.IsNotNull(prefab);
            if (!prefabToPool.TryGetValue(prefab, out GpuInstancerGroup pool))
            {
#if UNITY_EDITOR
                Assert.IsFalse(string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(prefab)), $"Объект '{prefab.name}' должен быть ассетом");
#endif
                pool = new GpuInstancerGroup(prefab, maxCount);
                prefabToPool.Add(prefab, pool);
            }

            pool.AddObject(position);
        }

        [Button]
        public void RemoveAll()
        {
            foreach ((GameObject prefab, GpuInstancerGroup pool) in prefabToPool)
                pool.RemoveAll();
        }

        public void Draw()
        {
            foreach ((GameObject prefab, GpuInstancerGroup pool) in prefabToPool)
                pool.Draw();
        }

        public void LogObjectsAmount()
        {
            prefabToPool.ToStringMultilineAndLog("Total blocks",
                pair => $"{pair.Value.Prefab.name} {pair.Value.ActualCount}");
        }
    }
}

