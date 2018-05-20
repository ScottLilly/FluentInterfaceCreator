using System.Collections.Generic;
using Engine.Models;

namespace Engine.Factories
{
    public static class OutputLanguageFactory
    {
        #region Variables with native datatypes for each language

        private static readonly List<Datatype> _nativeDatatypesForCSharp =
            new List<Datatype>
            {
                Datatype.BuildNativeDatatype("void", ""),
                Datatype.BuildNativeDatatype("byte", ""),
                Datatype.BuildNativeDatatype("sbyte", ""),
                Datatype.BuildNativeDatatype("short", ""),
                Datatype.BuildNativeDatatype("ushort", ""),
                Datatype.BuildNativeDatatype("int", ""),
                Datatype.BuildNativeDatatype("uint", ""),
                Datatype.BuildNativeDatatype("long", ""),
                Datatype.BuildNativeDatatype("ulong", ""),
                Datatype.BuildNativeDatatype("float", ""),
                Datatype.BuildNativeDatatype("double", ""),
                Datatype.BuildNativeDatatype("object", ""),
                Datatype.BuildNativeDatatype("char", ""),
                Datatype.BuildNativeDatatype("string", ""),
                Datatype.BuildNativeDatatype("decimal", ""),
                Datatype.BuildNativeDatatype("bool", ""),
                Datatype.BuildNativeDatatype("DateTime", "System")
            };

        #endregion

        public static List<OutputLanguage> GetLanguages()
        {
            return new List<OutputLanguage>
                   {
                       new OutputLanguage("C#", "cs", true, _nativeDatatypesForCSharp)
                   };
        }
    }
}