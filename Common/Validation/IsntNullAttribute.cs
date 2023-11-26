using System;

namespace GamePackages.Core.Validation
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class IsntNullAttribute : UnityEngine.PropertyAttribute
	{ 
		public bool Warring { get; set; }
	}
}