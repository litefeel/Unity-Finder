using System;

namespace litefeel.Finder.Editor
{
    [Flags]
    enum ShaderType
    {
        Project = 1 << 0,
        Package = 1 << 1,
        Builtin = 1 << 2,
        All = Project | Package | Builtin,
    }

    static class ShaderTypeExt
    {
        public static bool In(this ShaderType one, ShaderType all)
        {
            return (one & all) != 0;
        }
    }
}
