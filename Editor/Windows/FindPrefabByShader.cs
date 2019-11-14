using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByShader : FinderWindowBase<Shader, UnityEngine.Object>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
        }

        protected override bool InGameObject(GameObject prefab, Shader asset)
        {
            return FindUtil.InGameObject(prefab, asset);
        }

    }
}

