using System;
using System.Xml.Serialization;

namespace Engine.Models
{
    [Serializable]
    public class Datatype
    {
        public string Name { get; set; }
        public string InNamespace { get; set; }
        public bool IsNative { get; set; }

        [XmlIgnore]
        public bool CanDelete => !IsNative;

        public Datatype(string name, string inNamespace, bool isNative)
        {
            Name = name;
            InNamespace = inNamespace;
            IsNative = isNative;
        }

        // For serialization
        public Datatype()
        {
        }
    }
}