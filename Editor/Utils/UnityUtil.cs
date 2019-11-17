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
            while(trans != null)
            {
                list.Add(trans.name);
                trans = trans.parent;
            }
            list.Reverse();
            var fullpath = string.Join("/", list);
            ListPool<string>.Release(list);
            return fullpath;
        }


        public static bool AnyOneTransform(Func<Transform, bool> func, Transform root)
        {
            if (func(root)) return true;

            var count = root.childCount;
            for (var i = 0; i < count; i++)
            {
                if (AnyOneTransform(func, root.GetChild(i)))
                    return true;
            }
            return false;
        }

        public static bool AnyOneComponent(Func<Component, bool> func, Transform root, bool includeChildren = false)
        {
            var list = ListPool<Component>.Get();
            if (includeChildren)
                root.GetComponentsInChildren(true, list);
            else
                root.GetComponents(list);
            for (var i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                    return true;
            }
            return false;
        }
    }
}
