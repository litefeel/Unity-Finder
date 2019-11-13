using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByShader : FinderWindowBase<Shader, UnityEngine.Object>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
        }

        protected override void DoFind()
        {
            base.DoFind();
            m_Items.Clear();
            m_ItemNames.Clear();
            Finder.ForeachPrefabAndScene((obj, path) =>
            {
                bool has = false;
                switch (obj)
                {
                    case SceneAsset _:
                        has = InScene(path);
                        break;
                    case GameObject prefab:
                        has = InGameObject(prefab);
                        break;
                }
                if (has)
                {
                    m_Items.Add(obj);
                    m_ItemNames.Add(path);
                }
            }, true, GetSearchInFolders(), m_SearchType);
            m_SimpleTreeView.Reload();
        }

        private bool InScene(string scenePath)
        {
            bool has = false;
            Finder.ForeachRootGameObjectsInScene((go) =>
            {
                if (InGameObject(go))
                {
                    has = true;
                    return true;
                }
                return false;
            }, scenePath);
            return has;
        }
        private bool InGameObject(GameObject go)
        {
            foreach(var renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterials == null) continue;
                foreach(var mat in renderer.sharedMaterials)
                {
                    if (mat && mat.shader == m_Asset)
                        return true;
                }
            }
            return false;
        }

    }
}

