using System.Collections;
using UnityEngine;

namespace GamePackages.Core
{
	public static class AnimatorExtension
	{
		public static IEnumerator WaitForState(this Animator animator, string stateName, int layer = 0)
		{
			while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
				yield return null;
		}
	}
}