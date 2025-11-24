using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    internal class GpuInstancerGroup
    {
        readonly GameObject prefab;
        readonly Mesh mesh;
        readonly Material material;
        readonly MaterialPropertyBlock propertyBlock;
        readonly List<List<Matrix4x4>> matrices;
        readonly Quaternion rotation;
        readonly Vector3 scale;
        readonly int maxCount;

        public int ActualCount => matrices.Sum(x => x.Count);

        public GameObject Prefab => prefab;

        public GpuInstancerGroup(GameObject prefab, int maxCount)
        {
            Assert.IsNotNull(prefab);
            this.prefab = prefab;
            mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            material = prefab.GetComponent<MeshRenderer>().sharedMaterial;
            propertyBlock = new MaterialPropertyBlock();
            matrices = new List<List<Matrix4x4>>();
            matrices.Add(new List<Matrix4x4>(maxCount));
            rotation = prefab.transform.rotation;
            scale = prefab.transform.localScale;
            this.maxCount = maxCount;
            Assert.IsTrue(material.enableInstancing);
        }

        public bool IsPoolForPrefab(GameObject prefab)
        {
            return this.prefab == prefab;
        }

        public void AddObject(Vector3 position)
        {
            List<Matrix4x4> list = matrices[matrices.Count - 1];
            if (list.Count >= maxCount)
            {
                list = new List<Matrix4x4>(maxCount);
                matrices.Add(list);
            }

            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            list.Add(matrix);
        }

        public void RemoveAll()
        {
            matrices.Clear();
        }


        public void Draw()
        {
            if (matrices.Count == 0)
                return;

            // Отрисовываем все кубы за один вызов с помощью GPU Instancing
            foreach (var list in matrices)
                Graphics.DrawMeshInstanced(mesh, 0, material, list, propertyBlock);
        }
    }
}

