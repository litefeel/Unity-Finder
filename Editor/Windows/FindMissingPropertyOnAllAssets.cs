using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    class FindMissingPropertyOnAllAssets : FindWindowBase<UnityObject>
    {
        protected override bool InGameObject(GameObject prefab)
        {
            return UnityUtil.AnyOneTransform(FindUtil.CheckMissingPropOnTransfrom, prefab.transform);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

