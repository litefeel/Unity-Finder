using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    class FindMissingPropertyOnAllAssets : FindWindowBase<UnityObject>
    {
        protected override bool InGameObject(GameObject prefab)
        {
            var list = new List<Transform>();
            var has = ForeachTransform(prefab.transform, list);
            return has;
        }

        private bool ForeachTransform(Transform trans, List<Transform> list)
        {
            var comps = ListPool<Component>.Get();
            trans.GetComponents(comps);
            foreach (var comp in comps)
            {
                if (comp != null)
                {
                    var so = new SerializedObject(comp);
                    so.Update();
                    var prop = so.GetIterator();
                    if (CheckMissing(prop, true))
                        return true;
                }
            }
            var count = trans.childCount;
            for (var i = 0; i < count; i++)
            {
                if (ForeachTransform(trans.GetChild(i), list))
                    return true;
            }
            return false;
        }

        private bool CheckMissing(SerializedProperty prop, bool isFirst)
        {
            bool expanded = true;
            SerializedProperty end = null;
            if (!isFirst)
                end = prop.GetEndProperty();
            while (prop.NextVisible(expanded))
            {
                if (!isFirst && SerializedProperty.EqualContents(prop, end))
                    return false;
                if (prop.propertyType == SerializedPropertyType.ObjectReference
                    && EditorUtil.IsMissing(prop))
                    return true;

                if (CheckMissing(prop.Copy(), false))
                    return true;

                expanded = false;
            }
            return false;
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

