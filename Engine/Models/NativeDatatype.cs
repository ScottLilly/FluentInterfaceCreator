using System;

namespace Engine.Models
{
    [Serializable]
    public class NativeDatatype : Datatype
    {
        public NativeDatatype(string name) :
            base(name, true)
        {
        }

        // For serialization
        public NativeDatatype() : base("", true)
        {
        }
    }
}