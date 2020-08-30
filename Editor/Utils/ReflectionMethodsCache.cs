using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace UnityEngine.UI
{
    internal class ReflectionMethodsCache
    {
        public delegate string ToolbarSearchFieldCallback(string text, params GUILayoutOption[] options);
        public delegate RaycastHit2D Raycast2DCallback(Vector2 p1, Vector2 p2, float f, int i);
        public delegate RaycastHit[] RaycastAllCallback(Ray r, float f, int i);
        public delegate RaycastHit2D[] GetRayIntersectionAllCallback(Ray r, float f, int i);
        public delegate int GetRayIntersectionAllNonAllocCallback(Ray r, RaycastHit2D[] results, float f, int i);
        public delegate int GetRaycastNonAllocCallback(Ray r, RaycastHit[] results, float f, int i);

        // We call Physics.Raycast and Physics2D.Raycast through reflection to avoid creating a hard dependency from
        // this class to the Physics/Physics2D modules, which would otherwise make it impossible to make content with UI
        // without force-including both modules.
        public ReflectionMethodsCache()
        {
            var methodInfo = typeof(EditorGUILayout).GetMethod("ToolbarSearchField",
                BindingFlags.NonPublic | BindingFlags.Static, null,
                new[] { typeof(string), typeof(GUILayoutOption[]) }, null);
            if (methodInfo != null)
                ToolbarSearchField = (ToolbarSearchFieldCallback)Delegate.CreateDelegate(typeof(ToolbarSearchFieldCallback), methodInfo);

        }

        public ToolbarSearchFieldCallback ToolbarSearchField = null;

        private static ReflectionMethodsCache s_ReflectionMethodsCache = null;

        public static ReflectionMethodsCache Singleton
        {
            get
            {
                if (s_ReflectionMethodsCache == null)
                    s_ReflectionMethodsCache = new ReflectionMethodsCache();
                return s_ReflectionMethodsCache;
            }
        }
    };
}
