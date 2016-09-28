using System.Collections.Generic;

namespace Engine
{
    public static class LanguageFactory
    {
        public static List<string> PrimitiveDatatypeFor(Language language)
        {
            switch (language)
            {
                case Language.CSharp:
                    return DotNetPrimitiveDatatypes();
                default:
                    return new List<string>();
            }
        }

        private static List<string> DotNetPrimitiveDatatypes()
        {
            return new List<string>
            {
                "byte",
                "sbyte",
                "short",
                "ushort",
                "int",
                "uint",
                "long",
                "ulong",
                "float",
                "double",
                "object",
                "char",
                "string",
                "decimal",
                "bool",
                "DateTime",
                "DateSpan"
            };
        }
    }
}