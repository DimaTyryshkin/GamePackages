using GamePackages.Core;
using GamePackages.GamePackagesMath;
using NaughtyAttributes;
using UnityEngine;

namespace GamePackages.ForestGenerator
{
	public class R1QuasiRandomSequencesDrawer : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] GameObject[] prefabs;
		[SerializeField] int amount;
		[SerializeField] float scale;
		[SerializeField] float deviationRange;

		[SerializeField] float offset = 0.5f;
		
		[Space]
		[SerializeField] GameObject pointPrefabs;
		[SerializeField] int stepCount;
		[SerializeField] int randomStep;

		[Button()]
		void Build()
		{
			R1QuasiRandomSequences r1Sequences = new R1QuasiRandomSequences(offset);

			transform.DestroyChildren();
			//int totalN = Random.RandomRange(0,100000);
			int totalN = 0;
			int prefabNumber = 1;
			foreach (var prefab in prefabs)
			{
				for (int n = 0; n < amount /prefabNumber; n++)
				{
					Vector3 deviation = new Vector3(Random.Range(-deviationRange, deviationRange), 0, Random.Range(-deviationRange, deviationRange));
					
					Vector2 pos = r1Sequences.GetD2Float(totalN);
					var go = transform.InstantiateAsChild(prefab);
					go.transform.localPosition = new Vector3(pos.x * scale, 0, pos.y * scale) + deviation;
					totalN++;
				}

				prefabNumber++;
			}
		}


		int globalN = 0;
		R1QuasiRandomSequences r1Sequences2;
		[Button()]
		void Reset()
		{
			transform.DestroyChildren();
			globalN = 0;
			r1Sequences2 = new R1QuasiRandomSequences(offset);
		}

		[Button()]
		void AddPoint()
		{ 
			for (int n = 0; n < stepCount; n++)
			{ 
				var go = transform.InstantiateAsChild(pointPrefabs);
				//Vector3 posD3 = r1Sequences2.GetD3Float(globalN);
				//go.transform.localPosition = new Vector3(posD3.x * scale, posD3.y * scale, posD3.z * scale);
				
				Vector3 pos = r1Sequences2.GetD2Float(globalN);
				go.transform.localPosition = new Vector3(pos.x * scale, 0, pos.y * scale);
				globalN+=1 + Random.Range(0,randomStep);
			}
		}
#endif
	}
}