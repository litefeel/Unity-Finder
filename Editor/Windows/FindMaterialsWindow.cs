using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindMaterialsWindow : FinderWindowBase<Shader>
    {

        private readonly List<Material> m_Mats = new List<Material>();
        private string[] m_MatNames;

        protected override void OnGUI()
        {
            base.OnGUI();

            using (new EditorGUI.DisabledScope(m_Asset == null))
            {
                if (GUILayout.Button("Find"))
                    FindMaterials();
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


        private void FindMaterials()
        {
            m_Mats.Clear();
            Finder.FindMaterials(m_Asset, m_Mats);
            FillMatNames(m_Mats, ref m_MatNames);
            Debug.Log($"XXX {m_Mats.Count}");
        }

        private void FillMatNames(List<Material> mats, ref string[] matNames)
        {
            if (matNames == null || matNames.Length != mats.Count)
                matNames = new string[mats.Count];

            for (var i = 0; i < mats.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(mats[i]);
                if (!path.EndsWith(".mat"))
                    path += ".mat";
                matNames[i] = path;
            }
        }
    }
}

