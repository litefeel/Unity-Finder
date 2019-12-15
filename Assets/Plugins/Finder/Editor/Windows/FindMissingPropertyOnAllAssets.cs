using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    class FindMissingPropertyOnAllAssets : FindWindowBase<UnityObject>
    {
        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return UnityUtil.AnyOneComponentAndChildren<Component>(FindUtil.CheckMissingProp, prefab.transform);
        }
    }
}

