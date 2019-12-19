using System;
using System.Collections.Generic;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    internal static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, Clear);
        static void Clear(List<T> l) { l.Clear(); }

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }

    public struct ListPoolScope<T> : IDisposable
    {
        private bool m_Disposed;
        public readonly List<T> list;
        private ListPoolScope(List<T> list)
        {
            m_Disposed = false;
            this.list = list;
        }
        public void Dispose()
        {
            if (m_Disposed)
                return;
            m_Disposed = true;
            ListPool<T>.Release(list);
        }

        public static ListPoolScope<T> Create()
        {
            return new ListPoolScope<T>(ListPool<T>.Get());
        }
    }


}
