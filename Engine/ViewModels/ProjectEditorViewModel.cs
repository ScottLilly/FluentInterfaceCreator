using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Models;
using Engine.Resources;
using Engine.Utilities;

namespace Engine.ViewModels
{
    public class ProjectEditorViewModel : NotificationClassBase
    {
        #region Properties

        private Project _currentProject;
        private string _currentEditingMethodErrorMessage;
        private string _currentEditingMethodGroup;
        private string _currentEditingMethodName;
        private Method _currentMethod;

        public List<string> OutputLanguages { get; set; } =
            new List<string> {"C#"};

        public List<string> MethodGroups { get; set; } =
            new List<string> {"Instantiating", "Chaining", "Executing"};

        public bool HasProject => CurrentProject != null;
        public bool HasMethod => CurrentMethod != null;

        public Project CurrentProject
        {
            get { return _currentProject; }
            private set
            {
                if(_currentProject != value)
                {
                    _currentProject = value;

                    CurrentEditingMethodErrorMessage = "";
                    CurrentEditingMethodGroup = "";
                    CurrentEditingMethodName = "";
                    CurrentMethod = null;

                    NotifyPropertyChanged("CurrentProject");
                    NotifyPropertyChanged("HasProject");
                }
            }
        }

        public string CurrentEditingMethodErrorMessage
        {
            get { return _currentEditingMethodErrorMessage; }
            set
            {
                _currentEditingMethodErrorMessage = value;

                NotifyPropertyChanged(nameof(CurrentEditingMethodErrorMessage));
            }
        }

        public string CurrentEditingMethodGroup
        {
            get { return _currentEditingMethodGroup; }
            set
            {
                _currentEditingMethodGroup = value;

                NotifyPropertyChanged(nameof(CurrentEditingMethodGroup));
            }
        }

        public string CurrentEditingMethodName
        {
            get { return _currentEditingMethodName; }
            set
            {
                _currentEditingMethodName = value;

                NotifyPropertyChanged(nameof(CurrentEditingMethodName));
            }
        }

        public Method CurrentMethod
        {
            get { return _currentMethod; }
            set
            {
                _currentMethod = value;

                NotifyPropertyChanged(nameof(CurrentMethod));
                NotifyPropertyChanged(nameof(HasMethod));
            }
        }

        #endregion

        #region Public functions

        public void CreateNewProject()
        {
            CurrentProject = new Project();

            // This is currently the only output language, 
            // so select it by default.
            CurrentProject.OutputLanguage = "C#";
        }

        public void LoadProjectFromXML(string serializedProject)
        {
            CurrentProject = Serialization.Deserialize<Project>(serializedProject);
        }

        public void AddCurrentMethodToProject()
        {
            Method.MethodGroup group;
            Enum.TryParse(CurrentEditingMethodGroup, out group);

            List<string> errorMessages = new List<string>();

            // This null check is needed, 
            // because Enum.TryParse will set 'group' to 'Instantiating', 
            // when CurrentEditingMethodGroup is empty.
            if(CurrentEditingMethodGroup == null)
            {
                errorMessages.Add(ErrorMessages.GroupIsRequired);
            }

            if(group != Method.MethodGroup.Instantiating &&
               group != Method.MethodGroup.Chaining &&
               group != Method.MethodGroup.Executing)
            {
                errorMessages.Add(ErrorMessages.GroupIsNotValid);
            }

            if(string.IsNullOrWhiteSpace(CurrentEditingMethodName))
            {
                errorMessages.Add(ErrorMessages.NameIsRequired);
            }

            if(CurrentProject.AlreadyContainsMethodNamed(CurrentEditingMethodName))
            {
                errorMessages.Add(ErrorMessages.MethodAlreadyExists);
            }

            if(errorMessages.Any())
            {
                CurrentEditingMethodErrorMessage = string.Join("\r\n", errorMessages.ToArray());
                return;
            }

            CurrentEditingMethodErrorMessage = "";

            CurrentProject.AddMethod(new Method(group, CurrentEditingMethodName));

            // Clear input controls
            CurrentEditingMethodName = "";
            CurrentEditingMethodErrorMessage = "";
        }

        public void DeleteMethod(Method method)
        {
            CurrentProject.DeleteMethod(method);
        }

        public void SelectCallableMethodsAfter(Method method)
        {
            CurrentMethod = method;
        }

        // For now, instead of editing existing methods,
        // the user can delete and re-add.

        //public void EditMethod(Method method)
        //{
        //    CurrentEditingMethodGroup = method.Group.ToString();
        //    CurrentEditingMethodName = method.Name;
        //}

        #endregion
    }
}