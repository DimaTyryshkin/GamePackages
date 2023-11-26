using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using GamePackages.GamePackagesMath;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.ForestGenerator
{
	[System.Serializable]
    public class ForestEpoch  
    {
        public RangeMinMax age = new RangeMinMax(50, 150);
        public RangeMinMax scale = new RangeMinMax(0.8f, 1.3f);
        public RangeMinMax angle = new RangeMinMax(0, 360);
        public float density = 1;
  
        [ShowAssetPreview()]
        public GameObject[] objects;
    }
   
    
    public class ForestPointInfo
    {
	    public Vector3 point;
	    public float age;

	    public ForestPointInfo(Vector3 point, float age)
	    {
		    this.point = point;
		    this.age = age;
	    }
    }

    
    [AddComponentMenu("GamePackages/ForestGenerator/ForestBuilder")]
    /// <summary>
    /// В основе генератора лежит явление называемое 'Смена пород' – замена одних пород лесообразователей на другие на одной площади.
    /// Или в более общем виде 'Смена биоценозов'
    /// </summary>
    public class ForestBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] float ageToDistanceFactor = 5;
        [SerializeField] float forestAge = 50;
        [SerializeField] ForestSettings settings;
        [SerializeField, HideInInspector] Transform treeRoot;
      
        public IReadOnlyList<ForestPointInfo> Points => points;
        public Bounds Bounds { get; }

        ForestShape[] deForests;
        ForestShape[] forestSources;
        ForestShape[] borders;
        IReadOnlyList<ForestPointInfo> points;
        Bounds bounds;

        public void Init()
        {
	        var allElements = GetComponentsInChildren<ForestShape>();
	        foreach (var element in allElements)
		        element.FindPoints();
	        
	        deForests     = allElements.Where(x => x.type == ForestShape.ShapeType.deForest    ).ToArray();
	        forestSources = allElements.Where(x => x.type == ForestShape.ShapeType.forestSource).ToArray();
	        borders       = allElements.Where(x => x.type == ForestShape.ShapeType.border      ).ToArray();
 
	        AssertWrapper.IsAllNotNull(forestSources);
	        AssertWrapper.IsAllNotNull(borders);
	        
	        bounds = BoundsExtension.Encompass(borders.Select(x => x.GetBounds()).ToArray());
	        bounds = BoundsExtension.ToFlatXZ(bounds);
	        
	        points = GeneratePoints();
        }

        [Button()]
        public void BuildForest()
        {
	        Init();

	        if (treeRoot)
	        {
#if UNITY_EDITOR
		        Undo.DestroyObjectImmediate(treeRoot.gameObject);
#endif
		        if(treeRoot)
			        DestroyImmediate(treeRoot.gameObject);
	        }

	        if (!treeRoot)
	        {
		        GameObject root = new GameObject("treeRoot");
		        root.transform.SetParent(transform);

#if UNITY_EDITOR
		        Undo.RegisterCreatedObjectUndo(root, "create tree root");
		        Undo.RegisterCompleteObjectUndo(this, "create tree root");
#endif

		        treeRoot = root.transform;
	        }

	        foreach (var pointInfo in points)
		        BuildTree(pointInfo);
        }

        GameObject BuildTree(ForestPointInfo pointInfo)
        { 
	        int seed = Mathf.RoundToInt(pointInfo.point.x * 1000 + pointInfo.point.y * 1000 + bounds.size.z * 1000 + pointInfo.point.z * 10);
	        System.Random rnd = new  System.Random(seed);

	        float age = 0;
	        foreach (var epoch in settings.epochs)
	        {
		        float treeMaxAge = epoch.age.Random(rnd);
		      
		        if (pointInfo.age < age + treeMaxAge)
		        {
			        if (epoch.objects.Length > 0)
			        {
				        if (rnd.NextDouble() > epoch.density)
					        return null;
				        
				        float treeActualAge = pointInfo.age - age;
				        
				        GameObject prefab = epoch.objects.Random(rnd);
				        Transform newTree = treeRoot.InstantiateAsChild(prefab).transform;

#if UNITY_EDITOR
				        Undo.RegisterCompleteObjectUndo(newTree, "create tree root");
#endif 

				        // Scale
				        // 1)
				        //float scaleFactor = Mathf.Clamp01((treeActualAge / treeMaxAge) * 2f);// половину жизни дерево растет, а потом становится максимельного размера
				        //float maxScale = epoch.scale.Random(rnd);
				        //float minScale = maxScale * 0.3f;
				        
				        // 2)
				        //float scale = epoch.scale.Lerp(Mathf.Clamp01(treeActualAge / treeMaxAge));
				        //newTree.localScale = Vector3.one * scale;
				        
				        // 3)
				        float scale = epoch.scale.Random(rnd);
				        newTree.localScale = prefab.transform.localScale * scale;
				        
				        // Rotate and position
				        newTree.rotation = Quaternion.Euler(0, epoch.angle.Random(rnd), 0);
				        newTree.position = pointInfo.point;

				        return newTree.gameObject;
			        }
			        else
			        {
				        return null;
			        }
		        }
		        
		        age += treeMaxAge;
	        }

	        return null;
        }

        List<ForestPointInfo> GeneratePoints()
        {
	        float y = transform.position.y;
	        float area = bounds.size.x * bounds.size.z;
	        int plantAmount = Mathf.RoundToInt(area * settings.density);

	        List<ForestPointInfo> newPoints = new List<ForestPointInfo>(plantAmount);
	        R1QuasiRandomSequences r1QuasiRandomSequences = new R1QuasiRandomSequences(0.5f);

	        Debug.Log($"plantAmount={plantAmount}");
	        for (int i = 0; i < plantAmount; i++)
	        {
		        Vector2 point = r1QuasiRandomSequences.GetD2Float(i);
		        Vector3 xzPoint = new Vector3(point.x, 0, point.y);
		        Vector3 worldPoint = bounds.GetPointFormNormalizedPoint(xzPoint);
		        worldPoint.y = y;

		        ForestPointInfo pointInfo = GetForestAge(worldPoint);
		        if (pointInfo.age > 0)
			        newPoints.Add(pointInfo);
	        }

	        Debug.Log($"newPoints.Count={newPoints.Count}");
	        return newPoints;
        }

        ForestPointInfo GetForestAge(Vector3 point)
        {
	        float minDistance = borders.Min(x => x.GetDistance(point));
	        if (minDistance > 0)
	        {
		        return new ForestPointInfo(point, 0);
	        }
	        else
	        {
		        return new ForestPointInfo(point, GetForestAgeInternal(point));
	        }
        }

        float GetForestAgeInternal(Vector3 point)
        {
	        float minDistance = forestSources.Min(x => x.GetDistance(point));
	        float age = forestAge - minDistance / ageToDistanceFactor;

	        if (deForests.Length == 0)
		        return age;

	        age = deForests.Min(x => x.CorrectForestAge(point, age));
	        return age;
        } 
#endif
    } 
}