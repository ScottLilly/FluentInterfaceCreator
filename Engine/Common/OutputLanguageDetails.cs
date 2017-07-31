using System.Collections.Generic;
using Engine.Models;

namespace Engine.Common
{
    internal static class OutputLanguageDetails
    {
        internal static List<Datatype> NativeDatatypesFor(string language)
        {
            List<Datatype> nativeDatatypes = new List<Datatype>();

            if(language == "C#")
            {
                nativeDatatypes.Add(new Datatype("void", "", true));
                nativeDatatypes.Add(new Datatype("byte", "", true));
                nativeDatatypes.Add(new Datatype("sbyte", "", true));
                nativeDatatypes.Add(new Datatype("short", "", true));
                nativeDatatypes.Add(new Datatype("ushort", "", true));
                nativeDatatypes.Add(new Datatype("int", "", true));
                nativeDatatypes.Add(new Datatype("uint", "", true));
                nativeDatatypes.Add(new Datatype("long", "", true));
                nativeDatatypes.Add(new Datatype("ulong", "", true));
                nativeDatatypes.Add(new Datatype("float", "", true));
                nativeDatatypes.Add(new Datatype("double", "", true));
                nativeDatatypes.Add(new Datatype("object", "", true));
                nativeDatatypes.Add(new Datatype("char", "", true));
                nativeDatatypes.Add(new Datatype("string", "", true));
                nativeDatatypes.Add(new Datatype("decimal", "", true));
                nativeDatatypes.Add(new Datatype("bool", "", true));
                nativeDatatypes.Add(new Datatype("DateTime", "System", true));
            }

            return nativeDatatypes;
        }
    }
}