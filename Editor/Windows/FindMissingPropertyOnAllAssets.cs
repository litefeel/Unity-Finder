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

        private bool InMaterial(Material mat, Texture tex)
        {
            var so = new SerializedObject(mat);
            so.Update();
            var property = so.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference
                    && property.objectReferenceValue == tex)
                    return true;
            }
            return false;
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
                    var property = so.GetIterator();
                    bool expanded = true;
                    while (property.NextVisible(expanded))
                    {
                        if (property.propertyType == SerializedPropertyType.ObjectReference
                            && property.objectReferenceValue == null
                            && property.objectReferenceInstanceIDValue != 0)
                            return true;
                    }
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
        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

