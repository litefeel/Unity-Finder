using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    class SimpleTreeView : TreeView
    {
        
        private TreeViewItem m_Root = new TreeViewItem(-1, -1, "Root");
        private List<TreeViewItem> m_TreeItems = new List<TreeViewItem>();

        public Action<int> onItemSelect;
        public Action<int> onItemDoubleClick;

        private List<string> m_Items = new List<string>();
        public List<string> Items
        {
            get { return m_Items; }
            set { m_Items = value ?? new List<string>(); }
        }


        public SimpleTreeView(TreeViewState treeViewState)
        : base(treeViewState)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
            // are created from data. Here we create a fixed set of items. In a real world example,
            // a data model should be passed into the TreeView and the items created from the model.

            // This section illustrates that IDs should be unique. The root item is required to 
            // have a depth of -1, and the rest of the items increment from that.
            m_TreeItems.Clear();
            for (var i = 0; i < m_Items.Count; i++)
            {
                m_TreeItems.Add(new TreeViewItem(i, 0, m_Items[i]));
            }
            // Utility method that initializes the TreeViewItem.children and .parent for all items.
            SetupParentsAndChildrenFromDepths(m_Root, m_TreeItems);

            // Return root of the tree
            return m_Root;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void DoubleClickedItem(int id)
        {
            onItemDoubleClick?.Invoke(id);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            onItemSelect?.Invoke(selectedIds[0]);
        }
    }

}
