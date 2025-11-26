using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    public class GpuInstancer
    {
        Dictionary<GameObject, GpuInstancerGroup> prefabToPool = new Dictionary<GameObject, GpuInstancerGroup>();

        const int maxCount = 1023;

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

