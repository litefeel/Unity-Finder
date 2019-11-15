using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByShader : FindAssetWindowBase<Shader, UnityEngine.Object>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
        }

        protected override bool InGameObject(GameObject prefab)
        {
            return FindUtil.InGameObject(prefab, m_Asset);
        }

    }
}

