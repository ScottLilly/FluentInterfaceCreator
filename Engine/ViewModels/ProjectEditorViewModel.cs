using System.Collections.Generic;
using System.Linq;
using Engine.Models;

namespace Engine.ViewModels
{
    public class ProjectEditorViewModel : BaseNotificationClass
    {
        private Project _currentProject;
        private MethodAction _methodAction;
        private string _methodName;

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

        public Method SelectedMethod { get; set; }

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