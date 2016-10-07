using System.Collections.Generic;
using System.Collections.ObjectModel;
using Engine.Resources;

namespace Engine.Models
{
    public class Project : BaseNotificationClass
    {
        private bool _isComplete;
        private bool _isDirty;

        public List<MethodAction> Actions { get; } = 
            new List<MethodAction>
        {
            new MethodAction {ID = 1, Name = Literals.Instantiate},
            new MethodAction {ID = 2, Name = Literals.Continue},
            new MethodAction {ID = 3, Name = Literals.Execute}
        };

        public string Name { get; private set; }
        public Language OutputLanguage { get; private set; }

        public ObservableCollection<Method> Methods { get; }
        public ObservableCollection<Method> ChainableMethods { get; }

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
            ChainableMethods = new ObservableCollection<Method>();

            IsDirty = false;
            IsComplete = false;
        }

        public void AddMethod(Method method)
        {
            Methods.Add(method);

            if(method.ActionToPerform.Name == Literals.Continue ||
                method.ActionToPerform.Name == Literals.Execute)
            {
                ChainableMethods.Add(method);
            }

            IsDirty = true;
            IsComplete = false;
        }
    }
}