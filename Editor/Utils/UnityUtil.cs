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
    }
}
