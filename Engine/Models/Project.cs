using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.Models
{
    public class Project : BaseNotificationClass
    {
        private bool _isDirty;
        private bool _isComplete;

        public List<string> Actions => new List<string> { "Instantiate", "Continue", "Execute" };

        public string Name { get; private set; }
        public Language OutputLanguage { get; private set; }

        public ObservableCollection<Method> Methods { get; }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                _isDirty = value;

                NotifyPropertyChanged("IsDirty");
            }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            private set
            {
                _isComplete = value;
                
                NotifyPropertyChanged("IsComplete");
            }
        }

        public Project(string name, Language outputLanguage)
        {
            Name = name;
            OutputLanguage = outputLanguage;

            Methods = new ObservableCollection<Method>();

            IsDirty = false;
            IsComplete = false;
        }

        public void AddMethod(Method method)
        {
            Methods.Add(method);

            IsDirty = true;
            IsComplete = false;
        }
    }
}