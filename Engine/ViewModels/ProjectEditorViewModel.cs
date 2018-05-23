using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Engine.Factories;
using Engine.Models;
using Engine.Resources;
using Engine.Shared;
using PropertyChanged;

namespace Engine.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ProjectEditorViewModel
    {
        #region Properties

        public List<OutputLanguage> OutputLanguages { get; } = 
            OutputLanguageFactory.GetLanguages();

        private Project _currentProject;
        public Project CurrentProject
        {
            get => _currentProject;
            private set
            {
                if(_currentProject != value)
                {
                    _currentProject = value;

                    ClearViewModelProperties();
                }
            }
        }

        public string DatatypeUnderEditErrorMessage { get; set; }
        public Datatype DatatypeUnderEdit { get; private set; }

        public string MethodUnderEditErrorMessage { get; set; }
        public Method MethodUnderEdit { get; private set; }

        public string ParameterUnderEditErrorMessage { get; set; }
        public Parameter ParameterUnderEdit { get; private set; }

        public string InterfaceUnderEditErrorMessage { get; set; }
        public InterfaceData InterfaceUnderEdit { get; private set; }

        // TODO: Can we remove Selected<X> and use <X>UnderEdit?
        public Method SelectedMethod { get; set; }

        private InterfaceData _selectedInterface;
        public InterfaceData SelectedInterface
        {
            get => _selectedInterface;
            set
            {
                _selectedInterface = value;

                InterfaceUnderEditErrorMessage = "";
                InterfaceUnderEdit = SelectedInterface;
            }
        }

        public bool HasProject => CurrentProject != null;
        public bool HasMethod => SelectedMethod != null;
        public bool HasInterface => SelectedInterface != null;

        #endregion

        #region Public functions

        public void StartNewProject(OutputLanguage outputLanguage)
        {
            CurrentProject = new Project(outputLanguage);

            ClearViewModelProperties();
        }

        public void LoadProjectFromXML(string serializedProject)
        {
            // If the save file version matches the current app version, 
            // use the default XML deserializer.
            // If it is an older version, we need to manually deserialize.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(serializedProject);

            try
            {
                CurrentProject = Serialization.Deserialize<Project>(serializedProject);
            }
            catch
            {
                CurrentProject = new Project();
            }
        }

        public void AddNewDatatype()
        {
            List<string> errorMessages = DatatypeUnderEdit.ValidationErrors().ToList();

            if(CurrentProject.Datatypes.Any(dt => dt.Matches(DatatypeUnderEdit)))
            {
                errorMessages.Add(ErrorMessages.DatatypeAlreadyExists);
            }

            if(errorMessages.Any())
            {
                DatatypeUnderEditErrorMessage = errorMessages.ToStringWithLineFeeds();
                return;
            }

            CurrentProject.AddDatatype(DatatypeUnderEdit);

            ClearDatatypeUnderEdit();
        }

        public void DeleteDatatype(Datatype datatype)
        {
            CurrentProject.DeleteDatatype(datatype);
        }

        public void AddNewMethod()
        {
            List<string> errorMessages = MethodUnderEdit.ValidationErrors().ToList();

            if(CurrentProject.HasMethodWithMatchingSignature(MethodUnderEdit))
            {
                errorMessages.Add(ErrorMessages.MethodAlreadyExists);
            }

            if(errorMessages.Any())
            {
                MethodUnderEditErrorMessage = errorMessages.ToStringWithLineFeeds();
                return;
            }

            CurrentProject.AddMethod(MethodUnderEdit);

            ClearMethodUnderEdit();
        }

        public void DeleteMethod(Method method)
        {
            CurrentProject.DeleteMethod(method);
        }

        public void AddParameterToMethod()
        {
            List<string> errorMessages = ParameterUnderEdit.ValidationErrors().ToList();

            if(MethodUnderEdit.Parameters.Any(p => p.Matches(ParameterUnderEdit)))
            {
                errorMessages.Add(ErrorMessages.AnotherParameterHasThisName);
            }

            if(errorMessages.Any())
            {
                ParameterUnderEditErrorMessage = errorMessages.ToStringWithLineFeeds();
                return;
            }

            MethodUnderEdit.Parameters.Add(ParameterUnderEdit);

            ClearMethodParameterUnderEdit();
        }

        public void DeleteParameterFromMethod(Parameter parameter)
        {
            MethodUnderEdit.Parameters.Remove(parameter);
        }

        public void SelectMethodsCallableNextFor(Method method)
        {
            SelectedMethod = method;
        }

        public void UpdateSelectedInterfaceName()
        {
            List<string> errorMessages = InterfaceUnderEdit.ValidationErrors().ToList();

            if(InterfaceUnderEdit.Name.HasText() &&
               CurrentProject.Interfaces.Any(i => i.Matches(InterfaceUnderEdit) && i != SelectedInterface))
            {
                errorMessages.Add(ErrorMessages.AnotherInterfaceHasThisName);
            }

            if(errorMessages.Any())
            {
                InterfaceUnderEditErrorMessage = errorMessages.ToStringWithLineFeeds();
                return;
            }

            SelectedInterface.Name = InterfaceUnderEdit.Name;

            ClearInterfaceUnderEdit();
        }

        public void RefreshInterfaces()
        {
            CurrentProject.UpdateInterfaces();
        }

        public FluentInterfaceFile FluentInterfaceInSingleFile()
        {
            return FluentInterfaceCreatorFactory
                   .GetFluentInterfaceFileCreator(CurrentProject)
                   .CreateInSingleFile();
        }

        public IEnumerable<FluentInterfaceFile> FluentInterfaceInMultipleFiles()
        {
            return FluentInterfaceCreatorFactory
                   .GetFluentInterfaceFileCreator(CurrentProject)
                   .CreateInMultipleFiles();
        }

        #endregion

        #region Private functions

        private void ClearViewModelProperties()
        {
            ClearDatatypeUnderEdit();
            ClearMethodUnderEdit();
            ClearMethodParameterUnderEdit();
            ClearInterfaceUnderEdit();

            SelectedMethod = null;
            SelectedInterface = null;
        }

        private void ClearDatatypeUnderEdit()
        {
            DatatypeUnderEditErrorMessage = "";
            DatatypeUnderEdit = Datatype.BuildCustomDatatype("", "");
        }

        private void ClearMethodUnderEdit()
        {
            MethodUnderEditErrorMessage = "";
            MethodUnderEdit = new Method(MethodUnderEdit?.Group ?? Method.MethodGroup.Instantiating, "");
        }

        private void ClearMethodParameterUnderEdit()
        {
            ParameterUnderEditErrorMessage = "";
            ParameterUnderEdit = new Parameter();
        }

        private void ClearInterfaceUnderEdit()
        {
            InterfaceUnderEditErrorMessage = "";
            InterfaceUnderEdit = new InterfaceData();
        }

        #endregion
    }
}