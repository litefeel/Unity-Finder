using System.IO;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    public static class SelectionUtil
    {
        private static Object m_Selected;

        public static void Select(Object obj)
        {
            if (obj == null) return;
            if (obj == m_Selected) return;
            m_Selected = obj;
            EditorGUIUtility.PingObject(obj);
            //Selection.activeObject = obj;
        }

        public static void Clear()
        {
            m_Selected = null;
        }

        public static T GetAsset<T>() where T : UnityObject
        {
            var assets = Selection.GetFiltered<T>(SelectionMode.Assets);
            if (assets != null && assets.Length == 1)
                return assets[0];
            return null;
        }

        public static DefaultAsset GetSelectFolderAsset()
        {
            var asset = GetAsset<DefaultAsset>();
            if (asset != null)
            {
                if (Directory.Exists(AssetDatabase.GetAssetPath(asset)))
                    return asset;
            }
            return null;
        }
    }
}
