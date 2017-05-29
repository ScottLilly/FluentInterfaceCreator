using System;
using System.Collections.ObjectModel;

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

        public ObservableCollection<CallableMethodIndicator> MethodsCallableNext { get; set; } =
            new ObservableCollection<CallableMethodIndicator>();

        #endregion

        public Method(MethodGroup group, string name)
        {
            Group = group;
            Name = name;
        }

        // For serialization
        private Method()
        {
        }
    }
}