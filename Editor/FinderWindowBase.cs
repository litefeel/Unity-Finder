using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    abstract class FinderWindowBase<TAsset, TObject> : EditorWindow, IFinderWindow
        where TAsset : UnityObject
        where TObject : UnityObject
    {
        protected TAsset m_Asset;

        protected bool m_DisableFind;
        protected string m_Message;

        protected bool m_IgnoreSearchFolder;
        protected int m_FolderIdx;
        protected string[] m_FolderOptions = new string[] { "Assets", "Folder" };
        protected DefaultAsset m_Folder;

        protected List<string> m_ItemNames = new List<string>();
        protected List<TObject> m_Items = new List<TObject>();

        protected TreeViewState m_TreeViewState;
        protected SimpleTreeView m_SimpleTreeView;
        protected bool m_EnabledFindInScene;
        protected SearchType m_SearchType;

        private string m_FilterStr;

        public virtual void InitAsset(UnityObject obj)
        {
            m_Asset = obj as TAsset;
        }

        protected virtual void OnEnable()
        {
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();
            m_SimpleTreeView = new SimpleTreeView(m_TreeViewState);
            m_SimpleTreeView.onItemSelect = OnItemSelect;
            m_SimpleTreeView.onItemDoubleClick = OnItemDoubleClick;
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            m_Asset = EditorGUILayout.ObjectField("Asset", m_Asset, typeof(TAsset), false) as TAsset;
            if (Event.current.type == EventType.Layout)
                ConfigValues();
            OnGUIFindInScene();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(m_DisableFind))
            {
                GUILayoutOption[] options = null;
                if (!m_IgnoreSearchFolder)
                    options = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth) };
                if (GUILayout.Button("Find", options))
                    DoFind();
            }
            m_SearchType = (SearchType)EditorGUILayout.EnumPopup(m_SearchType);
            if (!m_IgnoreSearchFolder)
                OnGUISearchFolder();
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(m_Message))
                EditorGUILayout.HelpBox(m_Message, MessageType.Warning, true);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Count:{0}", m_ItemNames.Count));
            using (new EditorGUI.DisabledScope(m_ItemNames.Count == 0))
            {
                EditorGUILayout.LabelField("Filter:", GUILayout.Width(EditorUtil.CalcLabelSize("Filter:")));
                var tmpStr = EditorGUILayout.TextField(m_FilterStr);
                if (tmpStr != m_FilterStr)
                {
                    m_FilterStr = tmpStr;
                    m_SimpleTreeView.SetFilter(m_FilterStr);
                }
            }
            EditorGUILayout.EndHorizontal();

            m_SimpleTreeView.Items = m_ItemNames;
            var rect = GUILayoutUtility.GetRect(position.width, position.height);
            m_SimpleTreeView.OnGUI(rect);
        }

        protected virtual void ConfigValues() { }

        protected virtual void OnGUIFindInScene()
        {
            if (m_EnabledFindInScene)
            {
                if (GUILayout.Button("FindInScene", GUILayout.ExpandWidth(false)))
                    OnClickFindInScene();
            }
        }

        protected virtual string GetFindInSceneSearchFilter()
        {
            string searchFilter;

#if UNITY_2019_2_OR_NEWER
            // Don't remove "Assets" prefix, we need to support Packages as well (https://fogbugz.unity3d.com/f/cases/1161019/)
            string path = AssetDatabase.GetAssetPath(m_Asset);
#else
            // only main assets have unique paths (remove "Assets" to make string simpler)
            string path = AssetDatabase.GetAssetPath(m_Asset).Substring(7);
#endif
            if (path.IndexOf(' ') != -1)
                path = '"' + path + '"';

            if (AssetDatabase.IsMainAsset(m_Asset))
                searchFilter = "ref:" + path;
            else
                searchFilter = "ref:" + m_Asset.GetInstanceID() + ":" + path;

            return searchFilter;
        }

        protected virtual void OnClickFindInScene()
        {
            var searchFilter = GetFindInSceneSearchFilter();
            SearchableEditorWindowUtil.ForEach((win) =>
            {
                win.SetSearchFilter(searchFilter, SearchableEditorWindow.SearchMode.All);
            }, HierarchyType.GameObjects);
        }

        protected string[] GetSearchInFolders()
        {
            if (m_FolderIdx == 0 || m_Folder == null)
                return Array.Empty<string>();

            return new string[] { AssetDatabase.GetAssetPath(m_Folder) };
        }

        protected virtual void OnGUISearchFolder()
        {
            EditorGUILayout.LabelField(" Find From ", GUILayout.Width(70));

            m_FolderIdx = GUILayout.SelectionGrid(m_FolderIdx, m_FolderOptions, 2, EditorStyles.radioButton, GUILayout.ExpandWidth(false));
            using (new EditorGUI.DisabledScope(m_FolderIdx == 0))
                m_Folder = EditorGUILayout.ObjectField(m_Folder, typeof(DefaultAsset), false, GUILayout.ExpandWidth(true)) as DefaultAsset;
        }

        protected virtual void DoFind() { }

        protected virtual void OnItemSelect(int index)
        {
            SelectionUtil.Select(m_Items[index]);
        }

        protected virtual void OnItemDoubleClick(int index) { }
    }

}
