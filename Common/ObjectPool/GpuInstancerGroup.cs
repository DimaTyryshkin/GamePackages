using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    public struct GpuInstancerUnit
    {
        public GameObject prefab;
        public Mesh mesh;
        public Material material;
        public Quaternion prefabRotation;
        public Vector3 prefabScale;
    }

    internal class GpuInstancerGroup
    {
        readonly GameObject prefab;
        readonly Mesh mesh;
        readonly Material material;
        readonly List<List<Matrix4x4>> matrices;
        readonly Quaternion rotation;
        readonly Vector3 scale;
        readonly int maxCount;

        MaterialPropertyBlock propertyBlock;

        public int ActualCount => matrices.Sum(x => x.Count);

        public GameObject Prefab => prefab;

        public GpuInstancerGroup(GpuInstancerUnit prefab, int maxCount)
        {
            Assert.IsNotNull(prefab.prefab);
            this.prefab = prefab.prefab;
            mesh = prefab.mesh;// prefab.GetComponent<MeshFilter>().sharedMesh;
            material = prefab.material;// prefab.GetComponent<MeshRenderer>().sharedMaterial;
            //propertyBlock = new MaterialPropertyBlock();
            matrices = new List<List<Matrix4x4>>
            {
                new List<Matrix4x4>(maxCount)
            };
            rotation = prefab.prefabRotation;// prefab.transform.rotation;
            scale = prefab.prefabScale;// prefab.transform.localScale;
            this.maxCount = maxCount;

        }

        public void InitFromMainThread() // TODO
        {
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
            matrices.Add(new List<Matrix4x4>(maxCount));
        }


        public void Draw()
        {
            if (matrices.Count == 0)
                return;

            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();

            // Отрисовываем все кубы за один вызов с помощью GPU Instancing
            foreach (var list in matrices)
                Graphics.DrawMeshInstanced(mesh, 0, material, list, propertyBlock);
        }
    }
}

