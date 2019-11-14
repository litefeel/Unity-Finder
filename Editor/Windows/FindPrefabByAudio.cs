using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByAudio : FinderWindowBase<AudioClip, UnityEngine.Object>
    {

        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
        }

        protected override bool InGameObject(GameObject prefab, AudioClip m_Asset)
        {
            return FindUtil.InGameObject(prefab, m_Asset);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

