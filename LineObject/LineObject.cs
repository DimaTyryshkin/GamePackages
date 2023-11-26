using System;
using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.LineTool
{
    [ExecuteInEditMode]
    public class LineObject : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField, IsntNull] GameObject prefab;
        [SerializeField, IsntNull] GameObject lastPrefab;
        [SerializeField, IsntNull] Transform pointsRoot;
        [SerializeField] bool autoRebuild;
        [SerializeField] float space;
        [SerializeField] float yRotation;

        [SerializeField, HideInInspector] Transform objectsRoot;
        
        Transform[] pathPoints;
        Transform[] PathPoints => pathPoints;

        float timeNextRebuild;

        void Start()
        {
        }
 
        void OnEnable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        void OnDisable()
        {
            //Save();
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        
        void OnDrawGizmos()
        {
            if (!enabled)
                return;

            Validate();
            
            Vector3 p1 = PathPoints[0].position;
            for (int i = 1; i < PathPoints.Length; i++)
            {
               
                Vector3 p2 = PathPoints[i].position;
                Gizmos.DrawLine(p1, p2);
                p1 = p2;
            }
        }

        void OnSceneGUI(SceneView obj)
        {
            if (!enabled)
                return;

            Validate();

            for (int i = 0; i < PathPoints.Length; i++)
                PathPoints[i].position = Handles.PositionHandle(PathPoints[i].position, Quaternion.identity);
        }

        void Validate()
        { 
            if (autoRebuild)
            {
                if (Time.realtimeSinceStartup > timeNextRebuild)
                {
                    timeNextRebuild = Time.realtimeSinceStartup + 0.3f;
                    FindPoints();
                    Build();
                }
            }


            if (PathPoints == null || PathPoints.Any(x => !x))
                FindPoints();
        }

        [Button()]
        void Build()
        {
            Validate();
            
            if (objectsRoot)
            { 
                Undo.DestroyObjectImmediate(objectsRoot.gameObject);

                if(objectsRoot)
                    DestroyImmediate(objectsRoot.gameObject);
            }

            if (!objectsRoot)
            {
                GameObject root = new GameObject("root");
                root.transform.SetParent(transform);
 
                Undo.RegisterCreatedObjectUndo(root, "create root");
                Undo.RegisterCompleteObjectUndo(this, "create root"); 

                objectsRoot = root.transform;
            }
                 
            Bounds bounds = prefab.transform.GetTotalRendererBounds();
            float step = bounds.size.z + space;
            Vector3 point = pathPoints[0].position;

            int nextPointIndex = 1;
            GameObject newObject = null;
            while (nextPointIndex < pathPoints.Length)
            {
                Vector3 nextPoint = pathPoints[nextPointIndex].position;
                if (Vector3.Distance(point, nextPoint) >= step)
                {
                    Vector3 dir = nextPoint - point;
                    newObject = objectsRoot.InstantiateAsChild(prefab, localScaleToOne:false);
                    newObject.SetActive(true);
                    newObject.transform.rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0,yRotation,0);
                    newObject.transform.position = point;

                    point += dir.normalized * step;
                    //Undo.RegisterCreatedObjectUndo(newObject,"lineObject");
                }
                else
                {
                    nextPointIndex++;
                }
            }
            
            // replace with end
            if (lastPrefab && newObject)
            {
                var lastObject = objectsRoot.InstantiateAsChild(lastPrefab, localScaleToOne:false);
                lastObject.SetActive(true);
                lastObject.transform.rotation = newObject.transform.rotation;
                lastObject.transform.position = newObject.transform.position;
            } 
        }


        [Button()]
        void FindPoints()
        { 
            Undo.RecordObject(this, "Load"); 

            List<Transform> points = new List<Transform>();
            points.AddRange(pointsRoot.GetComponentsInChildren<Transform>());
            points.RemoveAt(0);

            pathPoints = points.ToArray();
        }

        [Button()]
        void Save()
        {
            foreach (var child in objectsRoot.GetComponentsInChildren<Transform>())
                Undo.RegisterCreatedObjectUndo(child.gameObject,"lineObject");
        }

#endif
    }
}