using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Engine.FluentInterfaceCreators;
using Engine.Resources;

namespace Engine.Models
{
    [Serializable]
    public class Project : NotificationClassBase
    {
        private readonly TextInfo _textInfo =
            new CultureInfo(CultureInfo.CurrentCulture.Name, true).TextInfo;

        #region Properties

        private string _factoryClassName;

        private string _name;
        private string _outputLanguage;
        private bool _isDirty;
        private string _fluentInterfaceAsOneFile;

        public string Name
        {
            get { return _name; }
            set
            {
                if(_name != value)
                {
                    SetDirty();
                }

                _name = value;

                NotifyPropertyChanged(nameof(Name));

                SetDefaultFactoryClassName("Builder");
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

        public string OutputLanguage
        {
            get { return _outputLanguage; }
            set
            {
                _outputLanguage = value;

                NotifyPropertyChanged(nameof(OutputLanguage));
            }
        }

        [XmlIgnore]
        public bool IncludeRegions { get; set; } = true;

        [XmlIgnore]
        public List<string> AdditionalReferences { get; set; } = new List<string>();

        public ObservableCollection<Method> InstantiatingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ChainingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ExecutingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<InterfaceData> Interfaces { get; set; } =
            new ObservableCollection<InterfaceData>();

        [XmlIgnore]
        public string FluentInterfaceAsOneFile
        {
            get { return _fluentInterfaceAsOneFile; }
            set
            {
                _fluentInterfaceAsOneFile = value; 
                
                NotifyPropertyChanged(nameof(FluentInterfaceAsOneFile));
            }
        }

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

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;

                NotifyPropertyChanged(nameof(IsDirty));
            }
        }

        #endregion

        #region Public functions

        public void AddMethod(Method methodToAdd)
        {
            AddMethodToCollection(methodToAdd);

            SetDirty();

            AddMethodAsCallableMethod(methodToAdd);

            AddChainEndingMethodsTo(methodToAdd);

            UpdateInterfaces();

            NotifyPropertyChanged(nameof(ChainStartingMethods));
            NotifyPropertyChanged(nameof(ChainEndingMethods));
        }

        private void AddMethodToCollection(Method method)
        {
            switch(method.Group)
            {
                case Method.MethodGroup.Instantiating:
                    InstantiatingMethods.Add(method);
                    break;
                case Method.MethodGroup.Chaining:
                    ChainingMethods.Add(method);
                    break;
                case Method.MethodGroup.Executing:
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
            if (methodToAdd.Group == Method.MethodGroup.Chaining ||
                methodToAdd.Group == Method.MethodGroup.Executing)
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
                                   cm.Name == chainEndingMethod.Name))
                {
                    method.MethodsCallableNext.Add(new CallableMethodIndicator(chainEndingMethod));
                }
            }
        }

        internal void UpdateInterfaces()
        {
            Interfaces.Clear();

            PopulateInterfacesForMethods(InstantiatingMethods);
            PopulateInterfacesForMethods(ChainingMethods);
        }

        private void PopulateInterfacesForMethods(IEnumerable<Method> methods)
        {
            foreach(Method method in methods
                .Where(m => !string.IsNullOrWhiteSpace(m.CallableMethodsSignature)))
            {
                InterfaceData interfaceData =
                    Interfaces.FirstOrDefault(i => i.CallableMethodsSignature == method.CallableMethodsSignature);

                if(interfaceData == null)
                {
                    interfaceData = new InterfaceData();

                    interfaceData.CalledByMethods.Add(method);

                    foreach (CallableMethodIndicator callableMethod in
                        method.MethodsCallableNext.Where(m => m.CanCall))
                    {
                        switch(callableMethod.Group)
                        {
                            case "Instantiating":
                                interfaceData
                                    .CallableMethods
                                    .Add(InstantiatingMethods
                                             .First(m => m.Name == callableMethod.Name));
                                break;
                            case "Chaining":
                                interfaceData
                                    .CallableMethods
                                    .Add(ChainingMethods
                                             .First(m => m.Name == callableMethod.Name));
                                break;
                            case "Executing":
                                interfaceData
                                    .CallableMethods
                                    .Add(ExecutingMethods
                                             .First(m => m.Name == callableMethod.Name));
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

        public void DeleteMethod(Method methodToRemove)
        {
            switch(methodToRemove.Group)
            {
                case Method.MethodGroup.Instantiating:
                    InstantiatingMethods.Remove(methodToRemove);
                    break;
                case Method.MethodGroup.Chaining:
                    ChainingMethods.Remove(methodToRemove);
                    break;
                case Method.MethodGroup.Executing:
                    ExecutingMethods.Remove(methodToRemove);
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.GroupIsNotValid);
            }

            SetDirty();

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

        private void AddMethodToCallableMethods(Method methodWithCallableMethods,
                                                Method methodToAdd)
        {
            if(!methodWithCallableMethods
                   .MethodsCallableNext
                   .Any(cm => cm.Group == methodToAdd.Group.ToString() &&
                              cm.Name == methodToAdd.Name))
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

            if(callableMethodToRemove != null)
            {
                methodWithCallableMethods.MethodsCallableNext.Remove(callableMethodToRemove);
            }
        }

        public bool AlreadyContainsMethodNamed(string methodName)
        {
            return
                InstantiatingMethods.Any(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase)) ||
                ChainingMethods.Any(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase)) ||
                ExecutingMethods.Any(m => m.Name.Equals(methodName, StringComparison.CurrentCultureIgnoreCase));
        }

        public void CreateFluentInterface()
        {
            IFluentInterfaceCreator creator = 
                FluentInterfaceCreatorFactory.GetCreatorForLanguage(OutputLanguage);

            // TODO: Add way to create single-file, or multiple file, fluent interfaces
            // Multiple file will put the interfaces in individual files.
            FluentInterfaceAsOneFile = creator.CreateFluentInterfaceFor(this);
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

        private void SetDirty()
        {
            IsDirty = true;
        }

        #endregion
    }
}