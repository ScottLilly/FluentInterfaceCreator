using System;
using System.Collections.Generic;
using Engine.Common;

namespace Engine.Models
{
    [Serializable]
    public class InterfaceData : NotificationClassBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                NotifyPropertyChanged(nameof(Name));
            }
        }

        public string CallableMethodsSignature { get; set; }

        public List<Method> CalledByMethods { get; set; } =
            new List<Method>();

        public List<Method> CallableMethods { get; set; } =
            new List<Method>();
    }
}