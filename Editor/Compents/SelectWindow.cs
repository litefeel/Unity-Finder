using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    class SelectWindow : EditorWindow
    {

        //[MenuItem("XXXXX/XXXX")]
        public static void XXXX()
        {
            var names = TypeCache.GetFullNames();
            var types = TypeCache.GetTypes();
            SelectWindow.get.Show(names, null, (index)=>
            {
                if(index >= 0 && index < types.Count)
                {
                    var type = types[index];
                    return type.FullName;
                }
                return "None";
            });
        }

        static SelectWindow s_SharedObjectSelector = null;
        public static SelectWindow get
        {
            get
            {
                if (s_SharedObjectSelector == null)
                {
                    UnityObject[] objs = Resources.FindObjectsOfTypeAll(typeof(SelectWindow));
                    if (objs != null && objs.Length > 0)
                        s_SharedObjectSelector = (SelectWindow)objs[0];
                    if (s_SharedObjectSelector == null)
                        s_SharedObjectSelector = ScriptableObject.CreateInstance<SelectWindow>();
                }
                return s_SharedObjectSelector;
            }
        }

        [SerializeField]
        protected TreeViewState m_TreeViewState;
        [SerializeField]
        protected SimpleTreeView m_SimpleTreeView;

        protected List<string> m_ItemNames = new List<string>();
        private string m_SearchFilter;
        private bool m_FocusSearchFilter;

        private int m_SelectedIdx = -1;

        private Action<int> m_OnSelectedCallback;
        private Func<int, string> m_DetailFunc;
        private float previewHeight = 20f;

        public void Show(List<string> list, Action<int> selectIndexCallback, Func<int, string> detailFunc = null)
        {
            m_OnSelectedCallback = selectIndexCallback;
            m_DetailFunc = detailFunc ?? DefaultDetail;

            m_ItemNames.AddRange(list);
            ShowAuxWindow();
            m_SimpleTreeView.Reload();
        }
        protected virtual void OnEnable()
        {
            titleContent = new GUIContent("Select");

            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();
            m_SimpleTreeView = new SimpleTreeView(m_TreeViewState);
            m_SimpleTreeView.onItemSelect = OnItemSelect;
            m_SimpleTreeView.onItemClick = OnItemClick;
            m_SimpleTreeView.onItemDoubleClick = OnItemDoubleClick;
            m_SimpleTreeView.Items = m_ItemNames;
            m_SimpleTreeView.Reload();
        }

        private void OnDisable()
        {
            m_ItemNames = null;
            m_DetailFunc = null;
            m_OnSelectedCallback = null;
        }

        protected virtual void OnItemSelect(int index)
        {
            m_SelectedIdx = index;
            m_OnSelectedCallback?.Invoke(index);
        }

        protected virtual void OnItemClick(int index)
        {
            m_OnSelectedCallback?.Invoke(index);
        }

        protected virtual void OnItemDoubleClick(int index)
        {
            m_OnSelectedCallback?.Invoke(index);
            Close();
        }


        private void OnGUI()
        {
            OnObjectGridGUI();
        }
        void OnObjectGridGUI()
        {
            //InitIfNeeded();

            //if (m_EditorCache == null)
            //    m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);

            //// Handle window/preview stuff
            //ResizeBottomPartOfWindow();

            Rect p = position;
            //EditorPrefs.SetFloat("ObjectSelectorWidth", p.width);
            //EditorPrefs.SetFloat("ObjectSelectorHeight", p.height);

            GUI.BeginGroup(new Rect(0, 0, position.width, position.height), GUIContent.none);

            // Let grid/list area take priority over search area on up/down arrow keys
            //if (s_GridAreaPriorityKeyboardEvents.Contains(Event.current))
            //    m_ListArea.HandleKeyboard(false);

            SearchArea();



            // Let grid/list area handle any keyboard events not used by search area
            //m_ListArea.HandleKeyboard(false);

            GridListArea();
            PreviewArea();

            GUI.EndGroup();

            //// overlay preview resize widget
            //GUI.Label(new Rect(position.width * .5f - 16, position.height - m_PreviewSize + 2, 32, Styles.bottomResize.fixedHeight), GUIContent.none, Styles.bottomResize);
        }

        private void PreviewArea()
        {
            GUI.Box(new Rect(0, position.height - previewHeight, position.width, previewHeight), "", "PopupCurveSwatchBackground");
            // Get info string
            var s = m_DetailFunc.Invoke(m_SelectedIdx) ?? "";
            Rect labelRect = new Rect(20, position.height - previewHeight + 1, position.width - 22, 18);
            if (EditorGUIUtility.isProSkin)
                EditorGUI.DropShadowLabel(labelRect, s, "ObjectPickerSmallStatus");
            else
                GUI.Label(labelRect, s, "ObjectPickerSmallStatus");
        }

        private void GridListArea()
        {
            var rect = new Rect(2, 0, position.width - 4, position.height - previewHeight);
            rect.y = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2) * 1;
            rect.height -= rect.y + EditorGUIUtility.standardVerticalSpacing;
            m_SimpleTreeView.SetFilter(m_SearchFilter);
            GUILayout.BeginArea(rect, EditorStyles.textField);
            {
                rect.position = new Vector2(0, 0);
                m_SimpleTreeView.OnGUI(rect);
            }
            GUILayout.EndArea();
        }

        void SearchArea()
        {
            // ESC clears search field and removes it's focus. But if we get an esc event we only want to clear search field.
            // So we need special handling afterwards.
            bool wasEscape = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;

            string searchFilter = ReflectionMethodsCache.Singleton.ToolbarSearchField(m_SearchFilter);

            if (wasEscape && Event.current.type == EventType.Used)
            {
                // If we hit esc and the string WAS empty, it's an actual cancel event.
                //if (m_SearchFilter == "")
                //    Cancel();

                // Otherwise the string has been cleared and focus has been lost. We don't have anything else to recieve focus, so we want to refocus the search field.
                m_FocusSearchFilter = true;
            }

            if (searchFilter != m_SearchFilter || m_FocusSearchFilter)
            {
                m_SearchFilter = searchFilter;
                //FilterSettingsChanged();
                // Repaint();
            }


            // TAB BAR
            //GUILayout.BeginArea(new Rect(4, kToolbarHeight, position.width - 4, kToolbarHeight));
            //GUILayout.BeginHorizontal();

            ////bool showingSceneTab = !m_IsShowingAssets;
            ////GUIContent sceneLabel = StageNavigationManager.instance.currentItem.isPrefabStage ? Styles.selfTabLabel : Styles.sceneTabLabel;
            ////showingSceneTab = GUILayout.Toggle(showingSceneTab, sceneLabel, Styles.tab);
            ////if (m_IsShowingAssets && showingSceneTab)
            ////    m_IsShowingAssets = false;



            //GUILayout.EndHorizontal();
            //GUILayout.EndArea();

            //if (GUI.changed)
            //    FilterSettingsChanged();

            //var size = new Vector2(0, 0);
            //if (m_IsShowingAssets)
            //{
            //    Styles.packagesVisibilityContent.text = PackageManagerUtilityInternal.HiddenPackagesCount.ToString();
            //    size = EditorStyles.toolbarButton.CalcSize(Styles.packagesVisibilityContent);
            //}

            //if (m_ListArea.CanShowThumbnails())
            //{
            //    EditorGUI.BeginChangeCheck();
            //    var newGridSize = (int)GUI.HorizontalSlider(new Rect(position.width - (60 + size.x), kToolbarHeight + GUI.skin.horizontalSlider.margin.top, 55, EditorGUI.kSingleLineHeight), m_ListArea.gridSize, m_ListArea.minGridSize, m_ListArea.maxGridSize);
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        m_ListArea.gridSize = newGridSize;
            //    }
            //}

            //if (m_IsShowingAssets)
            //{
            //    EditorGUI.BeginChangeCheck();
            //    var skipHiddenPackages = GUI.Toggle(new Rect(position.width - size.x, kToolbarHeight, size.x, EditorStyles.toolbarButton.fixedHeight), m_SkipHiddenPackages, Styles.packagesVisibilityContent, EditorStyles.toolbarButtonRight);
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        m_SkipHiddenPackages = skipHiddenPackages;
            //        FilterSettingsChanged();
            //    }
            //}
        }

        private string DefaultDetail(int index)
        {
            if (index >= 0 && m_ItemNames.Count > index)
            {
                return m_ItemNames[m_SelectedIdx];
                //string typeName = ObjectNames.NicifyVariableName(Name);
                //if (p != null)
                //    s = p.name + " (" + typeName + ")";
                //else
                //    s = selectedObject.name + " (" + typeName + ")";

                //s += "      " + AssetDatabase.GetAssetPath(selectedObject);
            }
            return "None";
        }
    }
}
