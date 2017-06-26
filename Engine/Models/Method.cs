using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Engine.Models
{
    [Serializable]
    public class Method : NotificationClassBase
    {
        public enum MethodGroup
        {
            Instantiating,
            Chaining,
            Executing
        }

        #region Properties

        private MethodGroup _group;
        private string _name;
        private string _returnDatatype;

        public MethodGroup Group
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
        public string ReturnDatatype
        {
            get { return _returnDatatype; }
            set
            {
                _returnDatatype = value;

                NotifyPropertyChanged(nameof(ReturnDatatype));
            }
        }

        public ObservableCollection<Parameter> Parameters { get; set; } =
            new ObservableCollection<Parameter>();

        public ObservableCollection<CallableMethodIndicator> MethodsCallableNext { get; set; } =
            new ObservableCollection<CallableMethodIndicator>();

        // Used to determine methods that can call the same 'next' methods,
        // to eliminate duplicate InterfaceData objects that identify the same interface.
        [XmlIgnore]
        public string CallableMethodsSignature =>
            string.Join("|", MethodsCallableNext
                .Where(x => x.CanCall)
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .Select(x => $"{x.Group}:{x.Name}"));

        #endregion

        public Method(MethodGroup group, string name, string returnDatatype = "")
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