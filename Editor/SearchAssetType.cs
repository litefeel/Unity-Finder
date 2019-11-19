using UnityEngine;

namespace litefeel.Finder.Editor
{
    public enum SearchAssetType
    {
        All,
        Prefab,
        Scene,
    }

    public enum SearchAssetFolder
    {
        Assets,
        [InspectorName("Assets & Packages")]
        AssetsAndPackages,
        Folder,
    }
}
