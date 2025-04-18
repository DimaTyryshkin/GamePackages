namespace GamePackages.Core
{
    public interface IPrepareAsset
    {
#if UNITY_EDITOR
        void OnPrepareAsset();
#endif
    }
}