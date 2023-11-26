using UnityEngine.Events;

namespace GamePackages.Core
{
	public static class ActionWrapper
	{
		public static void ClearAndInvoke(ref UnityAction action)
		{
			var copy = action;
			action = null;
			copy?.Invoke();
		}

		public static void ClearAndInvoke<T>(ref UnityAction<T> action, T arg)
		{
			var copy = action;
			action = null;
			copy?.Invoke(arg);
		}
	}
}