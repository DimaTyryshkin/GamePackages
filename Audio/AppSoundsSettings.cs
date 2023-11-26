using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePackages.Audio
{
	[CreateAssetMenu(fileName = "AppSoundsSettings", menuName = "GamePackages/Audio/AppSoundsSettings")]
	public class AppSoundsSettings : ScriptableObject
	{
		public SoundGroupSettings[] soundGroupSettings;
	}
	
	[Serializable]
	public struct SoundGroupSettings
	{
		public string name;
		public float minDelayToNextPlay;
	}
}
