using System;
using Engine.Common;

namespace Engine.Models
{
    [Serializable]
    public class Parameter : NotificationClassBase
    {
        private string _dataType;
        private string _name;
        private string _inNamespace;

        public string DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;

                NotifyPropertyChanged(nameof(DataType));
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

        public string InNamespace
        {
            get { return _inNamespace; }
            set
            {
                _inNamespace = value; 
                
                NotifyPropertyChanged(nameof(InNamespace));
            }
        }
    }
}