using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Engine.Common;
using Engine.FluentInterfaceCreators;
using Engine.Resources;

namespace Engine.Models
{
    [Serializable]
    public class Project : NotificationClassBase
    {
        private readonly TextInfo _textInfo =
            new CultureInfo(CultureInfo.CurrentCulture.Name, true).TextInfo;

        #region Events

        public event EventHandler FluentInterfaceFilesUpdated;

        #endregion

        #region Properties

        private string _name;
        private string _outputLanguage;
        private string _factoryClassNamespace;
        private string _factoryClassName;

        public ProjectVersion Version { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                NotifyPropertyChanged(nameof(Name));

                SetDefaultFactoryClassName("Builder");
            }
        }

        public string OutputLanguage
        {
            get { return _outputLanguage; }
            set
            {
                _outputLanguage = value;

                NotifyPropertyChanged(nameof(OutputLanguage));

                UpdateNativeDatatypes();
            }
        }

        public ObservableCollection<Datatype> Datatypes { get; set; } = 
            new ObservableCollection<Datatype>();

        public string FactoryClassNamespace
        {
            get { return _factoryClassNamespace; }
            set
            {
                _factoryClassNamespace = value; 
                
                NotifyPropertyChanged(nameof(FactoryClassName));
            }
        }

        public string FactoryClassName
        {
            get { return _factoryClassName; }
            set
            {
                _factoryClassName = value;

                NotifyPropertyChanged(nameof(FactoryClassName));
            }
        }

        public ObservableCollection<Method> InstantiatingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ChainingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ExecutingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<InterfaceData> Interfaces { get; set; } =
            new ObservableCollection<InterfaceData>();

        [XmlIgnore]
        public string InterfaceListAsCommaSeparatedString => 
            string.Join(", ", Interfaces.Select(i => i.Name));

        // For this section, a "chain" is two methods, called in order,
        // during the use of the fluent interface.

        // In these chains, 
        // the first method must be an InstantiatingMethod or a ChainingMethod. 
        // The second method must be a ChainingMethod or an ExecutingMethod.

        [XmlIgnore]
        public ObservableCollection<Method> ChainStartingMethods
        {
            get
            {
                ObservableCollection<Method> methods = new ObservableCollection<Method>();

                foreach(Method method in InstantiatingMethods)
                {
                    methods.Add(method);
                }

                foreach(Method method in ChainingMethods)
                {
                    methods.Add(method);
                }

                return methods;
            }
        }

        [XmlIgnore]
        public ObservableCollection<Method> ChainEndingMethods
        {
            get
            {
                ObservableCollection<Method> methods = new ObservableCollection<Method>();

                foreach(Method method in ChainingMethods)
                {
                    methods.Add(method);
                }

                foreach(Method method in ExecutingMethods)
                {
                    methods.Add(method);
                }

                return methods;
            }
        }

        [XmlIgnore]
        public ObservableCollection<FluentInterfaceFile> SingleFluentInterfaceFile { get; set; } =
            new ObservableCollection<FluentInterfaceFile>();

        [XmlIgnore]
        public ObservableCollection<FluentInterfaceFile> SeparateFluentInterfaceFiles { get; set; } = 
            new ObservableCollection<FluentInterfaceFile>();

        [XmlIgnore]
        public bool HasSingleFluentInterfaceFile => SingleFluentInterfaceFile.Any();

        [XmlIgnore]
        public bool HasSeparateFluentInterfaceFiles => SeparateFluentInterfaceFiles.Any();

        #endregion

        #region Constructors

        public Project() : this(Assembly.GetExecutingAssembly().GetName().Version.ToString())
        {
        }

        public Project(string version)
        {
            Version = new ProjectVersion(version);
        }

        #endregion

        #region Public functions

        public void DeleteDatatype(Datatype datatype)
        {
            Datatypes.Remove(datatype);
        }

        public void AddMethod(Method methodToAdd)
        {
            AddMethodToCollection(methodToAdd);
            AddMethodAsCallableMethod(methodToAdd);
            AddChainEndingMethodsTo(methodToAdd);

            UpdateInterfaces();

            NotifyPropertyChanged(nameof(ChainStartingMethods));
            NotifyPropertyChanged(nameof(ChainEndingMethods));
        }

        public void DeleteMethod(Method methodToRemove)
        {
            switch(methodToRemove.Group)
            {
                case Enums.MethodGroup.Instantiating:
                    InstantiatingMethods.Remove(methodToRemove);
                    break;
                case Enums.MethodGroup.Chaining:
                    ChainingMethods.Remove(methodToRemove);
                    break;
                case Enums.MethodGroup.Executing:
                    ExecutingMethods.Remove(methodToRemove);
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.GroupIsNotValid);
            }

            foreach(Method instantiatingMethod in InstantiatingMethods)
            {
                RemoveMethodFromCallableMethods(instantiatingMethod, methodToRemove);
            }

            foreach(Method chainingMethod in ChainingMethods)
            {
                RemoveMethodFromCallableMethods(chainingMethod, methodToRemove);
            }

            UpdateInterfaces();

            NotifyPropertyChanged(nameof(ChainStartingMethods));
            NotifyPropertyChanged(nameof(ChainEndingMethods));
        }

        public bool AlreadyContainsThisMethodSignature(string methodName, List<Parameter> parameters)
        {
            foreach(Method method in InstantiatingMethods
                .Where(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase) &&
                            m.Parameters.Count == parameters.Count))
            {
                if(parameters.Count == 0 ||
                   parameters.Where((t, i) => method.Parameters[i].DataType == t.DataType).Any())
                {
                    return true;
                }
            }

            foreach(Method method in ChainingMethods
                .Where(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase) &&
                            m.Parameters.Count == parameters.Count))
            {
                if(parameters.Count == 0 ||
                   parameters.Where((t, i) => method.Parameters[i].DataType == t.DataType).Any())
                {
                    return true;
                }
            }

            foreach(Method method in ExecutingMethods
                .Where(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase) &&
                            m.Parameters.Count == parameters.Count))
            {
                if(parameters.Count == 0 ||
                   parameters.Where((t, i) => method.Parameters[i].DataType == t.DataType).Any())
                {
                    return true;
                }
            }

            return false;
        }

        public void CreateFluentInterfaceFiles()
        {
            SingleFluentInterfaceFile.Clear();
            SeparateFluentInterfaceFiles.Clear();

            FluentInterfaceFileCreatorBase creator = 
                FluentInterfaceCreatorFactory.GetFluentInterfaceFileCreator(OutputLanguage, this);

            SingleFluentInterfaceFile.Add(creator.CreateSingleFile());
            
            IEnumerable<FluentInterfaceFile> files = creator.CreateIndividualFiles();

            foreach (FluentInterfaceFile fluentInterfaceFile in files)
            {
                SeparateFluentInterfaceFiles.Add(fluentInterfaceFile);
            }

            OnFluentInterfaceFilesUpdated();
        }

        #endregion

        #region Internal functions

        internal void UpdateNativeDatatypes()
        {
            // Remove existing native datatypes
            List<Datatype> nativeDatatypes =
                Datatypes.Where(d => d.IsNative).ToList();

            foreach (Datatype nativeDatatype in nativeDatatypes)
            {
                Datatypes.Remove(nativeDatatype);
            }

            // Insert native datatypes for current language
            foreach (Datatype nativeDatatype in
                OutputLanguageDetails.NativeDatatypesFor(OutputLanguage))
            {
                Datatypes.Add(nativeDatatype);
            }
        }

        internal void UpdateInterfaces()
        {
            Interfaces.Clear();

            PopulateInterfacesForMethods(InstantiatingMethods);
            PopulateInterfacesForMethods(ChainingMethods);
        }

        #endregion

        #region Private functions

        private void SetDefaultFactoryClassName(string suffix)
        {
            if(!string.IsNullOrWhiteSpace(Name) &&
               string.IsNullOrWhiteSpace(FactoryClassName))
            {
                FactoryClassName =
                    _textInfo.ToTitleCase(Name).Replace(" ", "") + suffix;
            }
        }

        private void AddMethodToCollection(Method method)
        {
            switch (method.Group)
            {
                case Enums.MethodGroup.Instantiating:
                    InstantiatingMethods.Add(method);
                    break;
                case Enums.MethodGroup.Chaining:
                    ChainingMethods.Add(method);
                    break;
                case Enums.MethodGroup.Executing:
                    ExecutingMethods.Add(method);
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.GroupIsNotValid);
            }
        }

        private void AddMethodAsCallableMethod(Method methodToAdd)
        {
            // Add this method as a callable method, to all ChainStarting methods,
            // if this is a ChainEnding method.
            if (methodToAdd.Group == Enums.MethodGroup.Chaining ||
                methodToAdd.Group == Enums.MethodGroup.Executing)
            {
                foreach (Method instantiatingMethod in InstantiatingMethods)
                {
                    AddMethodToCallableMethods(instantiatingMethod, methodToAdd);
                }

                foreach (Method chainingMethod in ChainingMethods)
                {
                    AddMethodToCallableMethods(chainingMethod, methodToAdd);
                }
            }
        }

        private void AddChainEndingMethodsTo(Method method)
        {
            // Add all ChainEndingMethods to this new Method
            foreach (Method chainEndingMethod in ChainEndingMethods)
            {
                if (!method
                        .MethodsCallableNext
                        .Any(cm => cm.Group == chainEndingMethod.Group.ToString() &&
                                   cm.DatatypeSignature == chainEndingMethod.DatatypeSignature))
                {
                    method.MethodsCallableNext.Add(new CallableMethodIndicator(chainEndingMethod));
                }
            }
        }

        private void PopulateInterfacesForMethods(IEnumerable<Method> methods)
        {
            foreach (Method method in methods
                .Where(m => !string.IsNullOrWhiteSpace(m.CallableMethodsSignature)))
            {
                InterfaceData interfaceData =
                    Interfaces.FirstOrDefault(i => i.CallableMethodsSignature == method.CallableMethodsSignature);

                if (interfaceData == null)
                {
                    interfaceData = new InterfaceData();

                    interfaceData.CalledByMethods.Add(method);

                    foreach (CallableMethodIndicator callableMethod in
                        method.MethodsCallableNext.Where(m => m.CanCall))
                    {
                        switch (callableMethod.Group)
                        {
                            case "Instantiating":
                                interfaceData
                                    .CallableMethods
                                    .Add(InstantiatingMethods
                                             .First(m => m.Signature == callableMethod.Signature));
                                break;
                            case "Chaining":
                                interfaceData
                                    .CallableMethods
                                    .Add(ChainingMethods
                                             .First(m => m.Signature == callableMethod.Signature));
                                break;
                            case "Executing":
                                interfaceData
                                    .CallableMethods
                                    .Add(ExecutingMethods
                                             .First(m => m.Signature == callableMethod.Signature));
                                break;
                        }
                    }

                    interfaceData.CallableMethodsSignature = method.CallableMethodsSignature;
                    Interfaces.Add(interfaceData);
                }
                else
                {
                    interfaceData.CalledByMethods.Add(method);
                }
            }
        }

        private void AddMethodToCallableMethods(Method methodWithCallableMethods,
                                                Method methodToAdd)
        {
            if (!methodWithCallableMethods
                    .MethodsCallableNext
                    .Any(cm => cm.Group == methodToAdd.Group.ToString() &&
                               cm.DatatypeSignature == methodToAdd.DatatypeSignature))
            {
                methodWithCallableMethods
                    .MethodsCallableNext
                    .Add(new CallableMethodIndicator(methodToAdd));
            }
        }

        private void RemoveMethodFromCallableMethods(Method methodWithCallableMethods,
                                                     Method methodToRemove)
        {
            CallableMethodIndicator callableMethodToRemove =
                methodWithCallableMethods
                    .MethodsCallableNext
                    .FirstOrDefault(cm => cm.Group == methodToRemove.Group.ToString() &&
                                          cm.Name == methodToRemove.Name);

            if (callableMethodToRemove != null)
            {
                methodWithCallableMethods.MethodsCallableNext.Remove(callableMethodToRemove);
            }
        }

        private void OnFluentInterfaceFilesUpdated()
        {
            FluentInterfaceFilesUpdated?.Invoke(this, new EventArgs());

            NotifyPropertyChanged(nameof(HasSingleFluentInterfaceFile));
            NotifyPropertyChanged(nameof(HasSeparateFluentInterfaceFiles));
        }

        #endregion
    }
}