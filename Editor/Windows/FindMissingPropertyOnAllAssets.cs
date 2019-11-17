using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    class FindMissingPropertyOnAllAssets : FindWindowBase<UnityObject>
    {
        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return UnityUtil.AnyOneTransformAndChildren(FindUtil.CheckMissingPropOnTransfrom, prefab.transform);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

