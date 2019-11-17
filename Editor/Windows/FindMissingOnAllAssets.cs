using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    using static UnityUtil;
    using static FindUtil;
    class FindMissingOnAllAssets : FindWindowBase<UnityObject>
    {
        protected override bool InGameObject(GameObject prefab)
        {
            return AnyOneTransform(CheckMissingScriptOnTransfrom, prefab.transform);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

