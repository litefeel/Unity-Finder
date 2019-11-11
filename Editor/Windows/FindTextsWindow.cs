using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace litefeel.Finder.Editor
{
    class FindTextsWindow : FinderWindowBase<Font>
    {

        private DefaultAsset m_Folder;
        private readonly List<GameObject> m_Prefabs = new List<GameObject>();
        private string[] m_PrefabNames;

        protected override void OnGUI()
        {
            base.OnGUI();
            //selection = AssetDatabase.LoadAssetAtPath(selectedPath, typeof(DefaultAsset)) as DefaultAsset;
            m_Folder = EditorGUILayout.ObjectField("Forld", m_Folder, typeof(DefaultAsset), false) as DefaultAsset;
            using (new EditorGUI.DisabledScope(m_Asset == null))
            {
                if (GUILayout.Button("Find"))
                    FindPrefabs();
            }
            var count = m_PrefabNames != null ? m_PrefabNames.Length : 0;
            EditorGUILayout.LabelField(string.Format("Count:{0}", count));
            if (count > 0)
            {
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                if (m_SelectedIdx >= m_PrefabNames.Length)
                    m_SelectedIdx = 0;
                m_SelectedIdx = GUILayout.SelectionGrid(m_SelectedIdx, m_PrefabNames, 1, EditorStyles.miniButton, GUILayout.ExpandHeight(false));
                EditorGUILayout.EndScrollView();

                SelectionUtil.Select(m_Prefabs[m_SelectedIdx]);
            }
        }


        private void FindPrefabs()
        {
            m_Prefabs.Clear();
            var texts = new List<Text>();

            string[] folder = null;
            if(m_Folder != null)
                folder = new string[] { AssetDatabase.GetAssetPath(m_Folder) };
            
            Finder.ForeachPrefabs((go, path)=> {
                texts.Clear();
                go.GetComponentsInChildren<Text>(true, texts);
                if (texts.Count > 0)
                    m_Prefabs.Add(go);
            }, true, folder);
            FillMatNames(m_Prefabs, ref m_PrefabNames);
            Debug.Log($"XXX {m_Prefabs.Count}");
        }

        private void FillMatNames(List<GameObject> mats, ref string[] matNames)
        {
            if (matNames == null || matNames.Length != mats.Count)
                matNames = new string[mats.Count];

            for (var i = 0; i < mats.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(mats[i]);
                matNames[i] = path;
            }
        }
    }
}

