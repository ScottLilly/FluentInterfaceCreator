using Engine.Models;

namespace Engine.ViewModels
{
    public class ProjectEditorViewModel : BaseNotificationClass
    {
        private Project _currentProject;

        public Project CurrentProject
        {
            get { return _currentProject; }
            private set
            {
                if(_currentProject != value)
                {
                    _currentProject = value;

                    OnPropertyChanged("CurrentProject");
                    OnPropertyChanged("HasProject");
                }
            }
        }

        public bool HasProject => CurrentProject != null;
        public bool HasChanges => (CurrentProject != null) && CurrentProject.IsDirty;

        public void CreateNewProject()
        {
            CurrentProject = new Project("", Language.CSharp);
        }
    }
}