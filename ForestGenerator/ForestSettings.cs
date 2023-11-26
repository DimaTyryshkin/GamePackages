using UnityEngine;

namespace GamePackages.ForestGenerator
{
	//[CreateAssetMenu]
	public class ForestSettings : ScriptableObject
	{
#if UNITY_EDITOR
		public float density;
		public ForestEpoch[] epochs;
#endif
	}
}