using UnityEditor;
using UnityEngine;

namespace GamePackages.Tools
{
    public static class UnityMenuEditor
    {
        [MenuItem("GamePackages/Alignment/AboveParent", false, 151)]
        public static void SetChildRendererAtCenter()
        {
            var all = Selection.gameObjects;

            foreach (var go in all)
            {
                Undo.RecordObject(go.transform, "AboveParent");
                Alignment.Alignment.AlignmentAboveParent(go.transform);
            }
        }

        [MenuItem("GamePackages/LightMapping/Enable generating lightmap data for all meshes in scene", false, 150)]
        public static void EnableGeneratingLightmapDataForAllMeshes()
        {
            Lightmapping.EnableGeneratingLightmapDataForAllMeshes();
        }

        [MenuItem("GamePackages/Alignment/RandomizeYRotation")]
        public static void RandomizeYRotation()
        {
            var all = Selection.gameObjects;

            foreach (var go in all)
            {
                Vector3 angles = go.transform.rotation.eulerAngles;
                angles.y = Random.Range(0, 360);
                Undo.RecordObject(go.transform, "Randomize");
                go.transform.rotation = Quaternion.Euler(angles);
            }
        }

        [MenuItem("GamePackages/Alignment/RandomizeScale")]
        public static void RandomizeScale()
        {
            var all = Selection.gameObjects;

            foreach (var go in all)
            {
                Vector3 scale = go.transform.localScale;
                scale *= Random.Range(0.8f, 1.3f);
                Undo.RecordObject(go.transform, "Randomize");
                go.transform.localScale = scale;
            }
        }

        [MenuItem("GamePackages/Alignment/RandomizeYRotationAndScale")]
        public static void RandomizeYRotationAndScale()
        {
            RandomizeScale();
            RandomizeYRotation();
        }
    }
}