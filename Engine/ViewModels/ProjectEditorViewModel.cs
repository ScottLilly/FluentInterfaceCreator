using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using Engine.Common;
using Engine.Models;
using Engine.Resources;
using Engine.Utilities;

namespace Engine.ViewModels
{
    public class ProjectEditorViewModel : NotificationClassBase
    {
        #region Properties

        private Project _currentProject;
        private string _currentEditingDatatypeErrorMessage;
        private string _currentEditingDatatypeName;
        private string _currentEditingDatatypeNamespace;
        private string _currentEditingMethodErrorMessage;
        private string _currentEditingMethodGroup;
        private string _currentEditingMethodName;
        private Datatype _currentEditingMethodReturnDatatype;
        private string _currentEditingMethodParameterErrorMessage;
        private Datatype _currentEditingMethodParameterDatatype;
        private string _currentEditingMethodParameterName;
        private Method _currentMethod;
        private string _currentEditingInterfaceErrorMessage;
        private string _currentEditingInterfaceName;
        private InterfaceData _currentInterface;

        public List<string> OutputLanguages { get; set; } =
            new List<string> {"C#"};

        public List<string> MethodGroups { get; set; } =
            new List<string> {"Instantiating", "Chaining", "Executing"};

        public bool HasProject => CurrentProject != null;
        public bool HasMethod => CurrentMethod != null;
        public bool HasInterface => CurrentInterface != null;
        public bool ShouldDisplayReturnDatatype => CurrentEditingMethodGroup == "Executing";

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
                    CurrentEditingDatatypeErrorMessage = "";
                    CurrentEditingDatatypeName = "";
                    CurrentEditingDatatypeNamespace = "";
                    CurrentEditingInterfaceErrorMessage = "";
                    CurrentEditingInterfaceName = "";
                    CurrentInterface = null;

                    NotifyPropertyChanged("CurrentProject");
                    NotifyPropertyChanged("HasProject");
                }
            }
        }

        public string CurrentEditingDatatypeErrorMessage
        {
            get { return _currentEditingDatatypeErrorMessage; }
            set
            {
                _currentEditingDatatypeErrorMessage = value; 
                
                NotifyPropertyChanged(nameof(CurrentEditingDatatypeErrorMessage));
            }
        }

        public string CurrentEditingDatatypeName
        {
            get { return _currentEditingDatatypeName; }
            set
            {
                _currentEditingDatatypeName = value; 
                
                NotifyPropertyChanged(nameof(CurrentEditingDatatypeName));
            }
        }

        public string CurrentEditingDatatypeNamespace
        {
            get { return _currentEditingDatatypeNamespace; }
            set
            {
                _currentEditingDatatypeNamespace = value; 
                
                NotifyPropertyChanged(nameof(CurrentEditingDatatypeNamespace));
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
                NotifyPropertyChanged(nameof(ShouldDisplayReturnDatatype));
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

        public Datatype CurrentEditingMethodReturnDatatype
        {
            get { return _currentEditingMethodReturnDatatype; }
            set
            {
                _currentEditingMethodReturnDatatype = value;

                NotifyPropertyChanged(nameof(CurrentEditingMethodReturnDatatype));
            }
        }

        public string CurrentEditingMethodParameterErrorMessage
        {
            get { return _currentEditingMethodParameterErrorMessage; }
            set
            {
                _currentEditingMethodParameterErrorMessage = value; 
                
                NotifyPropertyChanged(nameof(CurrentEditingMethodParameterErrorMessage));
            }
        }

        public ObservableCollection<Parameter> CurrentEditingMethodParameters { get; set; } =
            new ObservableCollection<Parameter>();

        public Datatype CurrentEditingMethodParameterDatatype
        {
            get { return _currentEditingMethodParameterDatatype; }
            set
            {
                _currentEditingMethodParameterDatatype = value; 
                
                NotifyPropertyChanged(nameof(CurrentEditingMethodParameterDatatype));
            }
        }

        public string CurrentEditingMethodParameterName
        {
            get { return _currentEditingMethodParameterName; }
            set
            {
                _currentEditingMethodParameterName = value; 
                
                NotifyPropertyChanged(nameof(CurrentEditingMethodParameterName));
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

        public string CurrentEditingInterfaceErrorMessage
        {
            get { return _currentEditingInterfaceErrorMessage; }
            set
            {
                _currentEditingInterfaceErrorMessage = value;

                NotifyPropertyChanged(nameof(CurrentEditingInterfaceErrorMessage));
            }
        }

        public string CurrentEditingInterfaceName
        {
            get { return _currentEditingInterfaceName; }
            set
            {
                _currentEditingInterfaceName = value;

                NotifyPropertyChanged(nameof(CurrentEditingInterfaceName));
            }
        }

        public InterfaceData CurrentInterface
        {
            get { return _currentInterface; }
            set
            {
                _currentInterface = value;

                if(CurrentInterface == null)
                {
                    CurrentEditingInterfaceName = null;
                    CurrentEditingInterfaceErrorMessage = null;
                }
                else
                {
                    CurrentEditingInterfaceName = CurrentInterface.Name;
                }

                NotifyPropertyChanged(nameof(CurrentInterface));
                NotifyPropertyChanged(nameof(HasInterface));
            }
        }

        #endregion

        #region Constructor

        public ProjectEditorViewModel()
        {
            // TODO: Find the reason for the null exception reference, and fix it.
            // This shouldn't be needed, but (somehow) I get a null exception without this.
            CurrentEditingMethodParameterName = "";
        }

        #endregion

        #region Public functions

        public void CreateNewProject()
        {
            CurrentProject = new Project();

            CurrentMethod = null;
            CurrentInterface = null;

            // This is currently the only output language, 
            // so select it by default.
            CurrentProject.OutputLanguage = "C#";
        }

        public void LoadProjectFromXML(string serializedProject)
        {
            // If the save file version matches the current app version, 
            // use the default XML deserializer.
            // If it is an older version, we need to manually deserialize.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(serializedProject);

            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;

            string major = doc.SelectSingleNode("/Project/Version/Major")?.InnerText ?? "0";
            string minor = doc.SelectSingleNode("/Project/Version/Minor")?.InnerText ?? "0";
            string revision = doc.SelectSingleNode("/Project/Version/Revision")?.InnerText ?? "0";
            string build = doc.SelectSingleNode("/Project/Version/Build")?.InnerText ?? "0";

            if(major == appVersion.Major.ToString() &&
               minor == appVersion.Minor.ToString() &&
               revision == appVersion.Revision.ToString() &&
               build == appVersion.Build.ToString())
            {
                CurrentProject = Serialization.Deserialize<Project>(serializedProject);
            }
            else
            {
                // TODO: Write deserializers for previous versions
                CurrentProject = new Project();
            }

            CurrentProject.UpdateNativeDatatypes();
        }

        public void AddNewDatatype()
        {
            List<string> errorMessages = new List<string>();

            if(string.IsNullOrWhiteSpace(CurrentEditingDatatypeName))
            {
                errorMessages.Add(ErrorMessages.DatatypeIsRequired);
            }

            if(CurrentProject.Datatypes.Any(dt => dt.Name == CurrentEditingDatatypeName &&
                                                  dt.InNamespace == CurrentEditingDatatypeNamespace.Trim()))
            {
                errorMessages.Add(ErrorMessages.DatatypeAlreadyExists);
            }

            if (!string.IsNullOrWhiteSpace(CurrentEditingDatatypeName) &&
                CurrentEditingDatatypeName.Trim().Contains(" "))
            {
                errorMessages.Add(ErrorMessages.DatatypeCannotContainAnInternalSpace);
            }

            if (!string.IsNullOrWhiteSpace(CurrentEditingDatatypeNamespace) &&
                CurrentEditingDatatypeNamespace.Trim().Contains(" "))
            {
                errorMessages.Add(ErrorMessages.NamespaceCannotContainAnInternalSpace);
            }

            if (errorMessages.Any())
            {
                CurrentEditingDatatypeErrorMessage = string.Join("\r\n", errorMessages.ToArray());
                return;
            }

            CurrentProject.Datatypes.Add(new Datatype(CurrentEditingDatatypeName, CurrentEditingDatatypeNamespace.Trim(), false));

            // Clear input controls
            CurrentEditingDatatypeErrorMessage = "";
            CurrentEditingDatatypeName = "";
            CurrentEditingDatatypeNamespace = "";
        }

        public void DeleteDatatype(Datatype datatype)
        {
            CurrentProject.DeleteDatatype(datatype);
        }

        public void AddParameterToMethod()
        {
            List<string> errorMessages = new List<string>();

            if(CurrentEditingMethodParameterDatatype == null)
            {
                errorMessages.Add(ErrorMessages.DatatypeIsRequired);
            }

            if(string.IsNullOrWhiteSpace(CurrentEditingMethodParameterName))
            {
                errorMessages.Add(ErrorMessages.NameIsRequired);
            }

            if(CurrentEditingMethodParameterName.Trim().Contains(" "))
            {
                errorMessages.Add(ErrorMessages.NameCannotContainAnInternalSpace);
            }

            if(CurrentEditingMethodParameters
                .Any(p => p.Name.Equals(CurrentEditingMethodParameterName.Trim())))
            {
                errorMessages.Add(ErrorMessages.AnotherParameterAlreadyHasThisName);
            }

            if(errorMessages.Any())
            {
                CurrentEditingMethodParameterErrorMessage =
                    string.Join("\r\n", errorMessages.ToArray());
                return;
            }

            CurrentEditingMethodParameters
                .Add(new Parameter
                     {
                         DataType = CurrentEditingMethodParameterDatatype.Name,
                         InNamespace = CurrentEditingMethodParameterDatatype.InNamespace,
                         Name = CurrentEditingMethodParameterName.Trim()
                     });

            // Clear input controls
            CurrentEditingMethodParameterErrorMessage = "";
            CurrentEditingMethodParameterDatatype = null;
            CurrentEditingMethodParameterName = "";
        }

        public void AddNewMethod()
        {
            List<string> errorMessages = new List<string>();

            if(string.IsNullOrWhiteSpace(CurrentEditingMethodGroup))
            {
                errorMessages.Add(ErrorMessages.GroupIsRequired);
            }
            else if(CurrentEditingMethodGroup != Enums.MethodGroup.Instantiating.ToString() &&
                    CurrentEditingMethodGroup != Enums.MethodGroup.Chaining.ToString() &&
                    CurrentEditingMethodGroup != Enums.MethodGroup.Executing.ToString())
            {
                // This error should never happen.
                // But, I'm adding it for safety.
                errorMessages.Add(ErrorMessages.GroupIsNotValid);
            }

            if(string.IsNullOrWhiteSpace(CurrentEditingMethodName))
            {
                errorMessages.Add(ErrorMessages.NameIsRequired);
            }

            if(CurrentEditingMethodName.Trim().Contains(" "))
            {
                errorMessages.Add(ErrorMessages.NameCannotContainAnInternalSpace);
            }

            if(CurrentProject.AlreadyContainsThisMethodSignature(CurrentEditingMethodName, CurrentEditingMethodParameters.ToList()))
            {
                errorMessages.Add(ErrorMessages.MethodAlreadyExists);
            }

            if (CurrentEditingMethodGroup == Enums.MethodGroup.Executing.ToString() &&
                CurrentEditingMethodReturnDatatype == null)
            {
                errorMessages.Add(ErrorMessages.ReturnTypeIsRequired);
            }

            if (errorMessages.Any())
            {
                CurrentEditingMethodErrorMessage = string.Join("\r\n", errorMessages.ToArray());
                return;
            }

            // If we got past the previous validations, this conversion should always succeed.
            Enums.MethodGroup group;
            Enum.TryParse(CurrentEditingMethodGroup, out group);

            Method methodToAdd =
                new Method(group,
                           CurrentEditingMethodName.Trim(),
                           CurrentEditingMethodGroup == "Executing"
                               ? CurrentEditingMethodReturnDatatype
                               : null);

            foreach(Parameter parameter in CurrentEditingMethodParameters)
            {
                methodToAdd.Parameters.Add(parameter);
            }

            CurrentProject.AddMethod(methodToAdd);

            // Clear input controls
            CurrentEditingMethodErrorMessage = "";
            CurrentEditingMethodName = "";
            CurrentEditingMethodParameters.Clear();
            CurrentEditingMethodParameterDatatype = null;
            CurrentEditingMethodParameterName = "";
        }

        public void DeleteMethod(Method method)
        {
            CurrentProject.DeleteMethod(method);
        }

        public void DeleteParameter(Parameter parameter)
        {
            CurrentEditingMethodParameters.Remove(parameter);
        }

        public void SelectMethodsCallableNextFor(Method method)
        {
            CurrentMethod = method;
        }

        public void RefreshCurrentProjectInterfaces()
        {
            CurrentProject.UpdateInterfaces();
        }

        public void SaveInterfaceName()
        {
            List<string> errorMessages = new List<string>();

            if(string.IsNullOrWhiteSpace(CurrentEditingInterfaceName))
            {
                errorMessages.Add(ErrorMessages.InterfaceNameIsRequired);
            }

            if(!string.IsNullOrWhiteSpace(CurrentEditingInterfaceName) &&
               CurrentProject.Interfaces.Any(interfaceData =>
                                                 interfaceData.Name == CurrentEditingInterfaceName &&
                                                 interfaceData != CurrentInterface))
            {
                errorMessages.Add(ErrorMessages.AnotherInterfaceAlreadyHasThisName);
            }

            if(CurrentEditingInterfaceName.Trim().Contains(" "))
            {
                errorMessages.Add(ErrorMessages.InterfaceNameCannotContainAnInternalSpace);
            }

            if (errorMessages.Any())
            {
                CurrentEditingInterfaceErrorMessage = string.Join("\r\n", errorMessages.ToArray());
                return;
            }

            CurrentEditingInterfaceErrorMessage = "";

            CurrentInterface.Name = CurrentEditingInterfaceName.Trim();

            // Clear input controls
            CurrentInterface = null;
        }

        public void CreateFluentInterface()
        {
            CurrentProject.CreateFluentInterfaceFiles();
        }

        #endregion
    }
}