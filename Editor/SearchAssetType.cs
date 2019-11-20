using UnityEngine;

#if !UNITY_2019_2_ORNEWER
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
