using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    abstract class FinderWindowBase<T> : EditorWindow, IFinderWindow where T : Object
    {
        protected T m_Asset;
        protected Vector2 m_ScrollPos = Vector2.zero;
        protected int m_SelectedIdx = 0;

        public virtual void InitAsset(Object obj)
        {
            m_Asset = obj as T;
        }

        protected virtual void OnGUI()
        {
            m_Asset = EditorGUILayout.ObjectField("Asset", m_Asset, typeof(T), false) as T;
        }

    }
    
}
