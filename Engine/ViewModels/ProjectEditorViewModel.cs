using System.Collections.Generic;
using System.Linq;
using Engine.Models;

namespace Engine.ViewModels
{
    public class ProjectEditorViewModel : BaseNotificationClass
    {
        private List<ChainableMethod> _chainableMethods;
        private Project _currentProject;
        private MethodAction _methodAction;
        private string _methodName;
        private Method _selectedMethod;

        public Project CurrentProject
        {
            get { return _currentProject; }
            private set
            {
                if(_currentProject != value)
                {
                    _currentProject = value;

                    NotifyPropertyChanged("CurrentProject");
                    NotifyPropertyChanged("HasProject");
                }
            }
        }

        public MethodAction MethodAction
        {
            get { return _methodAction; }
            set
            {
                _methodAction = value;

                NotifyPropertyChanged("MethodAction");
                NotifyPropertyChanged("CanAddMethod");
            }
        }

        public string MethodName
        {
            get { return _methodName; }
            set
            {
                _methodName = value;

                NotifyPropertyChanged("MethodName");
                NotifyPropertyChanged("CanAddMethod");
            }
        }

        public bool CanAddMethod => !string.IsNullOrWhiteSpace(MethodName) &&
                                    !string.IsNullOrWhiteSpace(MethodAction.Name);

        public Method SelectedMethod
        {
            get { return _selectedMethod; }
            set
            {
                _selectedMethod = value;

                if(_selectedMethod == null)
                {
                    ChainableMethods = null;
                }
                else
                {
                    if(SelectedMethod.ActionToPerform == Engine.Actions.Instantiate ||
                       SelectedMethod.ActionToPerform == Engine.Actions.Continue)
                    {
                        foreach(Method chainableMethod in CurrentProject.ChainableMethods)
                        {
                            _selectedMethod.AddChainableMethod(chainableMethod);
                        }
                    }

                    ChainableMethods = _selectedMethod.ChainableMethods
                                                      .OrderBy(x => x.Method.SortKey)
                                                      .ToList();
                }

                NotifyPropertyChanged("SelectedMethod");
            }
        }

        public List<ChainableMethod> ChainableMethods
        {
            get { return _chainableMethods; }
            set
            {
                _chainableMethods = value;

                NotifyPropertyChanged("ChainableMethods");
            }
        }

        public bool HasProject => CurrentProject != null;
        public bool HasChanges => (CurrentProject != null) && CurrentProject.IsDirty;
        public bool CanCreateFile => (CurrentProject != null) && CurrentProject.IsComplete;

        public List<MethodAction> Actions { get; set; } = new List<MethodAction>
                                                          {
                                                              Engine.Actions.Instantiate,
                                                              Engine.Actions.Continue,
                                                              Engine.Actions.Execute
                                                          };

        public void CreateNewProject()
        {
            CurrentProject = new Project("", Language.CSharp);
        }

        public void AddMethod()
        {
            if(_currentProject != null)
            {
                if(_currentProject.Methods.All(x => x.Name != MethodName))
                {
                    _currentProject.AddMethod(MethodAction, MethodName);

                    MethodName = "";
                }
            }
        }
    }
}