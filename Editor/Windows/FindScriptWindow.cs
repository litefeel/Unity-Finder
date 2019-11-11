using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace litefeel.Finder.Editor
{
    public class FindScriptWindow : EditorWindow
    {

        private MonoScript m_Script;
        private System.Type m_ScriptType;
        private List<GameObject> m_Mats = new List<GameObject>();
        private string[] m_MatNames;
        private Vector2 m_ScrollPos = Vector2.zero;
        private int m_SelectedIdx = 0;

        private void OnGUI()
        {
            m_Script = EditorGUILayout.ObjectField("Script", m_Script, typeof(MonoScript), false) as MonoScript;
            m_ScriptType = null;
            if (m_Script != null)
            {
                m_ScriptType = m_Script.GetClass();
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
            var count = m_MatNames != null ? m_MatNames.Length : 0;
            EditorGUILayout.LabelField(string.Format("Count:{0}", count));
            if (count > 0)
            {
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                if (m_SelectedIdx >= m_MatNames.Length)
                    m_SelectedIdx = 0;
                m_SelectedIdx = GUILayout.SelectionGrid(m_SelectedIdx, m_MatNames, 1, EditorStyles.miniButton, GUILayout.ExpandHeight(false));
                EditorGUILayout.EndScrollView();

                SelectionUtil.Select(m_Mats[m_SelectedIdx]);
            }
        }


        private List<Component> m_Images = new List<Component>();
        private void Find()
        {
            m_Mats.Clear();
            Finder.ForeachPrefabs((prefab, path) =>
            {
                m_Images.Clear();
                var componets = prefab.GetComponentsInChildren(m_ScriptType, true);
                if (componets != null && componets.Length > 0)
                    m_Mats.Add(prefab);
            }, true);
            FillNames(m_Mats, ref m_MatNames);
            Debug.Log($"XXX {m_Mats.Count}");
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

