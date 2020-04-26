using System;
using System.Collections.Generic;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class TypeCache
    {
        private static List<Type> s_TypeList;
        private static List<string> s_NameList;

        public static List<Type> GetTypes()
        {
            Init();
            return s_TypeList;
        }
        public static List<string> GetNames()
        {
            Init();
            return s_NameList;
        }

        private static void Init()
        {
            if (s_TypeList != null && s_NameList != null) return;

            s_TypeList = new List<Type>();
            s_NameList = new List<string>();


            var typeSets = new HashSet<Type>();
            var compType = typeof(MonoBehaviour);
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyName = assembly.GetName().FullName;
                if (assemblyName == "UnityEditor" || assemblyName.StartsWith("UnityEditor.") ||
                    assemblyName == "System" || assemblyName.StartsWith("System.") ||
                    assemblyName == "mscorlib" || assemblyName.StartsWith("Microsoft."))
                    continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type == compType)
                        continue;
                    if (compType.IsAssignableFrom(type) && !typeSets.Contains(type))
                    {
                        typeSets.Add(type);
                        s_TypeList.Add(type);
                        s_NameList.Add(type.Name);
                    }
                }
            }

            //Debug.LogError(string.Join("\n", s_NameList));
        }
    }
}
