using UnityEngine.Events;
using GamePackages.Localization.EndingOfNumerals;

namespace GamePackages.Localization
{
	public static class LocalizationFacade
	{
		public static event UnityAction Reloaded;
		
		public static void Reload()
		{
			LocalizedKeyString.Reload();
			EndingOfDatesStore.Reset();
         
			Reloaded?.Invoke();
		}
	}
}