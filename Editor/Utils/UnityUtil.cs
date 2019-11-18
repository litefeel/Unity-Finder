using System;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class UnityUtil
    {

        public static string GetFullPath(Transform trans)
        {
            var list = ListPool<string>.Get();
            while (trans != null)
            {
                list.Add(trans.name);
                trans = trans.parent;
            }
            list.Reverse();
            var fullpath = string.Join("/", list);
            ListPool<string>.Release(list);
            return fullpath;
        }
        public static bool AnyOneMaterialAndChildren(Func<Material, bool> func, GameObject go)
        {
            return AnyOneComponentAndChildren<Renderer>((renderer) =>
            {
                var sharedMaterials = renderer.sharedMaterials;
                if (sharedMaterials == null) return false;
                foreach (var mat in sharedMaterials)
                {
                    if (func(mat))
                        return true;
                }
                return false;
            }, go.transform);
        }
        public static bool AnyOneProperty(Func<SerializedProperty, bool> func, UnityEngine.Object obj)
        {
            if (obj == null) return false;
            var so = new SerializedObject(obj);
            so.Update();
            //Debug.Log(obj);
            var prop = so.GetIterator();
            return AnyOneProperty(func, prop, true);
        }
        public static bool AnyOneProperty(Func<SerializedProperty, bool> func, SerializedProperty prop, bool isFirst)
        {
            bool expanded = true;
            SerializedProperty end = null;
            if (!isFirst)
                end = prop.GetEndProperty();
            while (prop.NextVisible(expanded))
            {
                expanded = false;
                if (!isFirst && SerializedProperty.EqualContents(prop, end))
                    return false;
                var propType = prop.propertyType;
                //Debug.Log(prop.propertyPath);
                if (propType != SerializedPropertyType.ObjectReference &&
                    propType != SerializedPropertyType.Generic)
                    continue;
                if (func(prop))
                    return true;

                if (AnyOneProperty(func, prop.Copy(), false))
                    return true;

            }
            return false;
        }

        public static bool AnyOneTransformAndChildren(Func<Transform, bool> func, Transform root)
        {
            if (func(root)) return true;

            var count = root.childCount;
            for (var i = 0; i < count; i++)
            {
                if (AnyOneTransformAndChildren(func, root.GetChild(i)))
                    return true;
            }
            return false;
        }

        public static bool AnyOneComponent<T>(Func<T, bool> func, Transform root)
            where T : Component
        {
            using (var scope = ListPoolScope<T>.Create())
            {
                root.GetComponents(scope.list);
                for (var i = 0; i < scope.list.Count; i++)
                {
                    if (func(scope.list[i]))
                        return true;
                }
            }
            return false;
        }

        public static bool AnyOneComponentAndChildren<T>(Func<T, bool> func, Transform root)
            where T : Component
        {
            using (var scope = ListPoolScope<T>.Create())
            {
                root.GetComponentsInChildren(scope.list);
                for (var i = 0; i < scope.list.Count; i++)
                {
                    if (func(scope.list[i]))
                        return true;
                }
            }
            return false;
        }
    }
}
