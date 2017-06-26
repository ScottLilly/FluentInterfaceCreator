using System.Collections.Generic;
using Engine.Models;

namespace Engine.Utilities
{
    internal static class OutputLanguageDetails
    {
        internal static List<Datatype> NativeDatatypesFor(string language)
        {
            List<Datatype> nativeDatatypes = new List<Datatype>();

            if(language == "C#")
            {
                nativeDatatypes.Add(new NativeDatatype("void"));
                nativeDatatypes.Add(new NativeDatatype("byte"));
                nativeDatatypes.Add(new NativeDatatype("sbyte"));
                nativeDatatypes.Add(new NativeDatatype("short"));
                nativeDatatypes.Add(new NativeDatatype("ushort"));
                nativeDatatypes.Add(new NativeDatatype("int"));
                nativeDatatypes.Add(new NativeDatatype("uint"));
                nativeDatatypes.Add(new NativeDatatype("long"));
                nativeDatatypes.Add(new NativeDatatype("ulong"));
                nativeDatatypes.Add(new NativeDatatype("float"));
                nativeDatatypes.Add(new NativeDatatype("double"));
                nativeDatatypes.Add(new NativeDatatype("object"));
                nativeDatatypes.Add(new NativeDatatype("char"));
                nativeDatatypes.Add(new NativeDatatype("string"));
                nativeDatatypes.Add(new NativeDatatype("decimal"));
                nativeDatatypes.Add(new NativeDatatype("bool"));
                nativeDatatypes.Add(new NativeDatatype("DateTime"));
            }

            return nativeDatatypes;
        }
    }
}