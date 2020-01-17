using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindUsage : FindAssetWindowBase<UnityEngine.Object, UnityEngine.Object>
    {
        readonly Type m_UnityObjectType = typeof(UnityEngine.Object);
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
            var list = GetSearchInFolders();
            if(list.Length > 0)
            {
                string prefix = list[0];
                var tmp = new List<string>(paths);
                tmp.RemoveAll((f) => !f.StartsWith(prefix));
                paths = tmp.ToArray();
            }
            //paths = new string[] { "Assets/Art/FBmap/Materials/7.mat", "Assets/BuildOnlyAssets/FX/Materials/Eagle_high_fx.mat" };
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
                //Debug.Log(ext + " " + path);
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var asset in assets)
                {
                    // asset is null when missing script
                    if (asset == null) continue;
                    if (asset == m_Asset) continue;
                    if (asset.GetType() == m_UnityObjectType)
                    {
                        // UnityObject cannot contain other asset
                        continue;
                        //Debug.Log($"{asset}, {asset.GetType()}, {asset.name}, {path}");
                        // Ignore Deprecated EditorExtensionImpl
                        //if (asset.name == "Deprecated EditorExtensionImpl")
                        //    continue;
                    }

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
                if (FindUtil.IgnoreType(comp)) return false;
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

