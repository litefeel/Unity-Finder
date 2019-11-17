using System;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindUsage : FindAssetWindowBase<UnityEngine.Object, UnityEngine.Object>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
        }

        protected override void DoFind()
        {
            m_Items.Clear();
            m_ItemNames.Clear();
            var time = DateTime.Now;
            var paths = AssetDatabase.GetAllAssetPaths();
            //paths = new string[] { "Assets/Animation/Animation.unity" };
            Finder.Progress((path) =>
            {
                var ext = System.IO.Path.GetExtension(path);
                switch (ext.ToLower())
                {
                    case ".cs": return;
                    case ".unity":
                        if (FindUtil.InScene(path, InGameObjectAndChildren))
                        {
                            m_Items.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
                        }
                        return;
                }
                //Debug.Log(ext + " "+path);
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var asset in assets)
                {
                    if (asset == m_Asset) continue;
                    if (InAsset(asset, path))
                        m_Items.Add(asset);
                }
            }, paths);
            Debug.Log(DateTime.Now - time);
            m_ItemNames.Clear();
            foreach (var item in m_Items)
                m_ItemNames.Add(AssetDatabase.GetAssetPath(item));
            m_SimpleTreeView.Reload();
        }

        private bool InAsset(UnityEngine.Object asset, string path)
        {
            switch (asset)
            {
                case GameObject go:
                    return InGameObjectAndChildren(go);
                case SceneAsset scene:
                    return FindUtil.InScene(path, InGameObjectAndChildren);
                default:
                    return UnityUtil.AnyOneProperty((prop) =>
                    {
                        return prop.propertyType == SerializedPropertyType.ObjectReference &&
                        prop.objectReferenceValue == m_Asset;

                    }, asset);
            }
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return UnityUtil.AnyOneComponentAndChildren<Component>((comp) =>
            {
                return UnityUtil.AnyOneProperty((prop) =>
                {
                    return prop.propertyType == SerializedPropertyType.ObjectReference &&
                    prop.objectReferenceValue == m_Asset;

                }, comp);
            }, prefab.transform);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }
    }
}

