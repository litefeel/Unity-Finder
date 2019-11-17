using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByAudio : FindAssetWindowBase<AudioClip, UnityEngine.Object>
    {

        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return FindUtil.InGameObject(prefab, m_Asset);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

