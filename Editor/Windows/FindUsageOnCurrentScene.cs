using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindUsageOnCurrentScene : FindAssetWindowBase<UnityEngine.Object, Transform>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = false;
            m_IgnoreSearchAssetFolder = true;
            m_IgnoreScearchAssetType = true;
        }

        protected override void DoFind()
        {
            m_Items.Clear();
            m_ItemNames.Clear();

            FindUtil.FilterCurrentStageOrScene(InGameObjectAndChildren, m_Items);

            foreach (var item in m_Items)
            {
                m_ItemNames.Add(UnityUtil.GetFullPath(item));
            }

            m_SimpleTreeView.Reload();
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return InGameObjectAndChildren(prefab.transform);
        }

        protected bool InGameObjectAndChildren(Transform prefab)
        {
            return UnityUtil.AnyOneComponent<Component>((comp) =>
            {
                if (FindUtil.IgnoreType(comp)) return false;
                return UnityUtil.AnyOneProperty((prop) =>
                {
                    return prop.propertyType == SerializedPropertyType.ObjectReference &&
                    prop.objectReferenceValue == m_Asset;

                }, comp);
            }, prefab);
        }
    }
}

