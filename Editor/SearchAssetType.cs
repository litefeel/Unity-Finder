using UnityEngine;

#if !UNITY_2019_2_OR_NEWER
using InspectorName = System.ComponentModel.DescriptionAttribute;
#endif
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
        [InspectorName("Assets + Packages")]
        AssetsAndPackages,
        Folder,
    }
}
