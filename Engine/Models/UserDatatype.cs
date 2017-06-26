using System;

namespace Engine.Models
{
    [Serializable]
    public class UserDatatype : Datatype
    {
        public UserDatatype(string name) :
            base(name, false)
        {
        }

        // For serialization
        public UserDatatype() : base("", false)
        {
        }
    }
}