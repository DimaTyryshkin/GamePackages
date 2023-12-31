using GamePackages.Core;
using UnityEngine;

namespace GamePackages.Core
{
	public interface IAssetBundle
	{
		string BundleName { get; }
	
		T[]              LoadAllAssets<T>() where T : Object;
		T                LoadAsset<T>(string      name) where T : Object;
		bool             Contains(string          name);
		ProcessStatus<T> LoadAssetAsync<T>(string name) where T : Object;
	}
}