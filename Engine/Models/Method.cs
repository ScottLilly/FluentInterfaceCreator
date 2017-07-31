using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Engine.Common;

namespace Engine.Models
{
    [Serializable]
    public class Method : NotificationClassBase
    {
        private Enums.MethodGroup _group;
        private string _name;
        private Datatype _returnDatatype;

        public Enums.MethodGroup Group
        {
            get { return _group; }
            set
            {
                _group = value;

                NotifyPropertyChanged(nameof(Group));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                NotifyPropertyChanged(nameof(Name));
            }
        }

        // Only used for Executing methods
        public Datatype ReturnDatatype
        {
            get { return _returnDatatype; }
            set
            {
                _returnDatatype = value;

                NotifyPropertyChanged(nameof(ReturnDatatype));
            }
        }

        [XmlIgnore]
        public List<string> NamespacesNeeded
        {
            get
            {
                List<string> namespacesNeeded = new List<string>();

                namespacesNeeded.AddRange(Parameters.Select(parameter => parameter.InNamespace));
                namespacesNeeded.Add(ReturnDatatype?.InNamespace);

                return namespacesNeeded.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
            }
        }

        public string ParameterList => 
            string.Join(", ", Parameters.Select(p => $"{p.DataType} {p.Name}"));

        public string ParameterDatatypeList =>
            string.Join(", ", Parameters.Select(p => $"{p.DataType}"));

        public ObservableCollection<Parameter> Parameters { get; set; } =
            new ObservableCollection<Parameter>();

        public ObservableCollection<CallableMethodIndicator> MethodsCallableNext { get; set; } =
            new ObservableCollection<CallableMethodIndicator>();

        [XmlIgnore]
        public string Signature => $"{Name}({ParameterList})";

        [XmlIgnore]
        public string DatatypeSignature => $"{Name}({ParameterDatatypeList})";

        // Used to determine methods that can call the same 'next' methods,
        // to eliminate duplicate InterfaceData objects that identify the same interface.
        [XmlIgnore]
        public string CallableMethodsSignature =>
            string.Join("|", MethodsCallableNext
                .Where(x => x.CanCall)
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .Select(x => $"{x.Group}:{x.Name}"));

        public Method(Enums.MethodGroup group, string name, Datatype returnDatatype = null)
        {
            Group = group;
            Name = name;
            ReturnDatatype = returnDatatype;
        }

        // For serialization
        private Method()
        {
        }
    }
}