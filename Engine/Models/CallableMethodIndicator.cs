using System;

namespace Engine.Models
{
    [Serializable]
    public class CallableMethodIndicator : NotificationClassBase
    {
        #region Properties

        private string _group;
        private string _name;
        private bool _canCall;

        public string Group
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

        public bool CanCall
        {
            get { return _canCall; }
            set
            {
                _canCall = value;

                NotifyPropertyChanged(nameof(CanCall));
            }
        }

        #endregion

        public CallableMethodIndicator(Method method, bool canCall = false)
        {
            Group = method.Group.ToString();
            Name = method.Name;
            CanCall = canCall;
        }

        // For serialization
        private CallableMethodIndicator()
        {
        }
    }
}