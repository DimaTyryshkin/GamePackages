 

namespace GamePackages.Core
{
	public interface IDependencyFinder
	{
#if UNITY_EDITOR
		void FindDependency();
#endif
	}
}