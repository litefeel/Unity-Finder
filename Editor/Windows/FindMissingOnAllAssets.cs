using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    using static UnityUtil;
    class FindMissingOnAllAssets : FindWindowBase<UnityObject>
    {
        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return AnyOneComponentAndChildren<Component>(comp =>
            {
                return comp == null;
            }, prefab.transform);
        }
    }
}

