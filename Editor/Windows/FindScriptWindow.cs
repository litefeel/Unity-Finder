using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace litefeel.Finder.Editor
{
    class FindScriptWindow : FinderWindowBase<MonoScript>
    {
        private System.Type m_ScriptType;
        private List<GameObject> m_Assets = new List<GameObject>();
        private string[] m_AssetNames;

        protected override void OnGUI()
        {
            base.OnGUI();
            
            m_ScriptType = null;
            if (m_Asset != null)
            {
                m_ScriptType = m_Asset.GetClass();
                if (!typeof(Component).IsAssignableFrom(m_ScriptType))
                {
                    m_ScriptType = null;
                    EditorGUILayout.HelpBox("Script must be  inherit from UnityEngine.Component", MessageType.Warning, true);
                }
            }
            
            using (new EditorGUI.DisabledScope(m_ScriptType == null))
            {
                if (GUILayout.Button("Find"))
                    Find();
            }
            var count = m_AssetNames != null ? m_AssetNames.Length : 0;
            EditorGUILayout.LabelField(string.Format("Count:{0}", count));
            if (count > 0)
            {
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                if (m_SelectedIdx >= m_AssetNames.Length)
                    m_SelectedIdx = 0;
                m_SelectedIdx = GUILayout.SelectionGrid(m_SelectedIdx, m_AssetNames, 1, EditorStyles.miniButton, GUILayout.ExpandHeight(false));
                EditorGUILayout.EndScrollView();

                SelectionUtil.Select(m_Assets[m_SelectedIdx]);
            }
        }


        private List<Component> m_Images = new List<Component>();
        private void Find()
        {
            m_Assets.Clear();
            Finder.ForeachPrefabs((prefab, path) =>
            {
                m_Images.Clear();
                var componets = prefab.GetComponentsInChildren(m_ScriptType, true);
                if (componets != null && componets.Length > 0)
                    m_Assets.Add(prefab);
            }, true);
            FillNames(m_Assets, ref m_AssetNames);
            Debug.Log($"XXX {m_Assets.Count}");
        }

        private void FillNames(List<GameObject> mats, ref string[] matNames)
        {
            if (matNames == null || matNames.Length != mats.Count)
                matNames = new string[mats.Count];

            for (var i = 0; i < mats.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(mats[i]);
                //if (!path.EndsWith(".mat"))
                //    path += ".mat";
                matNames[i] = path;
            }
        }
    }
}

