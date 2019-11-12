using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    abstract class FinderWindowBase<T> : EditorWindow, IFinderWindow where T : UnityObject
    {
        protected T m_Asset;
        protected Vector2 m_ScrollPos = Vector2.zero;
        protected int m_SelectedIdx = 0;

        protected bool m_DisableFind;
        protected string m_Message;

        public virtual void InitAsset(UnityObject obj)
        {
            m_Asset = obj as T;
        }

        protected virtual void OnGUI()
        {
            m_Asset = EditorGUILayout.ObjectField("Asset", m_Asset, typeof(T), false) as T;
            using(new EditorGUI.DisabledScope(m_DisableFind))
            {
                if (GUILayout.Button("Find"))
                    DoFind();
            }
            if (!string.IsNullOrEmpty(m_Message))
                EditorGUILayout.HelpBox(m_Message, MessageType.Warning, true);
        }

        protected virtual void DoFind()
        {
            
        }
    }
    
}
