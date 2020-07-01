using System;
using System.Collections.Generic;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class TypeCache
    {
        private static List<Type> s_TypeList;
        private static List<string> s_FullNameList;
        private static Dictionary<string, Type> s_NameTypeMap;
        private static Dictionary<string, Type> s_FullNameTypeMap;

        public static List<Type> GetTypes()
        {
            Init();
            return s_TypeList;
        }
        public static List<string> GetFullNames()
        {
            Init();
            return s_FullNameList;
        }
        public static Type FindType(string fullnameOrName)
        {
            if (string.IsNullOrEmpty(fullnameOrName))
                return null;

            Init();
            Type type;
            if (s_FullNameTypeMap.TryGetValue(fullnameOrName, out type))
                return type;

            s_NameTypeMap.TryGetValue(fullnameOrName, out type);
            return type;
        }
        public static Type GetTypeByFullName(string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                return null;
            Init();
            s_FullNameTypeMap.TryGetValue(fullname, out var type);
            return type;
        }

        public static Type GetTypeByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            Init();
            s_NameTypeMap.TryGetValue(name, out var type);
            return type;
        }

        private static void Init()
        {
            if (s_TypeList != null && s_FullNameList != null) return;

            s_TypeList = new List<Type>();
            s_FullNameList = new List<string>();
            s_NameTypeMap = new Dictionary<string, Type>();
            s_FullNameTypeMap = new Dictionary<string, Type>();


            var typeSet = new HashSet<Type>();
            var nameSet = new HashSet<string>();
            var compType = typeof(Component);
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
                    if (compType.IsAssignableFrom(type) && !typeSet.Contains(type))
                    {
                        var fullname = type.FullName;
                        var name = type.Name;

                        typeSet.Add(type);
                        s_TypeList.Add(type);
                        s_FullNameList.Add(fullname);
                        s_FullNameTypeMap.Add(fullname, type);

                        if (nameSet.Add(name))
                            s_NameTypeMap.Add(name, type);
                        else
                            s_NameTypeMap.Remove(name);
                    }
                }
            }

            //Debug.LogError(string.Join("\n", s_NameList));
        }
    }
}
