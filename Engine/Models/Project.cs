using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Engine.Resources;
using Engine.Shared;
using PropertyChanged;

namespace Engine.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Project
    {
        #region Properties

        public OutputLanguage OutputLanguage { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                SetDefaultFactoryClassName();
            }
        }

        public string FactoryClassNamespace { get; set; }

        public string FactoryClassName { get; set; }

        public ObservableCollection<Datatype> Datatypes { get; } =
            new ObservableCollection<Datatype>();

        public ObservableCollection<Method> InstantiatingMethods { get; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ChainingMethods { get; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ExecutingMethods { get; } =
            new ObservableCollection<Method>();

        public ObservableCollection<InterfaceData> Interfaces { get; } =
            new ObservableCollection<InterfaceData>();

        public string InterfaceListAsCommaSeparatedString => 
            string.Join(", ", Interfaces.Select(i => i.Name));

        // For this section, a "chain" is two methods, called in order,
        // during the use of the fluent interface.
        // The first method must be an InstantiatingMethod or a ChainingMethod. 
        // The second method must be a ChainingMethod or an ExecutingMethod.
        public ObservableCollection<Method> ChainStartingMethods { get; } = 
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ChainEndingMethods { get; } =
            new ObservableCollection<Method>();

        public bool CanCreateFiles => FactoryClassName.HasText();

        #endregion

        #region Constructors

        public Project(OutputLanguage outputLanguage)
        {
            OutputLanguage = outputLanguage;

            foreach(Datatype datatype in outputLanguage.NativeDatatypes)
            {
                AddDatatype(datatype);
            }
        }

        // For Serialization
        internal Project()
        {
        }

        #endregion

        #region Public functions

        public void AddDatatype(Datatype datatype)
        {
            Datatypes.Add(datatype);
        }

        public void DeleteDatatype(Datatype datatype)
        {
            Datatypes.Remove(datatype);
        }

        public void AddMethod(Method method)
        {
            AddMethodToCollection(method);
            AddMethodAsCallableMethod(method);
            AddChainEndingMethodsTo(method);

            UpdateInterfaces();
        }

        public bool HasMethodWithMatchingSignature(Method method)
        {
            return InstantiatingMethods.Any(m => m.Matches(method, OutputLanguage.IsCaseSensitive)) ||
                   ChainEndingMethods.Any(m => m.Matches(method, OutputLanguage.IsCaseSensitive)) ||
                   ExecutingMethods.Any(m => m.Matches(method, OutputLanguage.IsCaseSensitive));
        }

        public void DeleteMethod(Method method)
        {
            switch (method.Group)
            {
                case Method.MethodGroup.Instantiating:
                    InstantiatingMethods.Remove(method);
                    ChainStartingMethods.Remove(method);
                    break;
                case Method.MethodGroup.Chaining:
                    ChainingMethods.Remove(method);
                    ChainStartingMethods.Remove(method);
                    ChainEndingMethods.Remove(method);
                    break;
                case Method.MethodGroup.Executing:
                    ExecutingMethods.Remove(method);
                    ChainEndingMethods.Remove(method);
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.GroupIsNotValid);
            }

            foreach(Method instantiatingMethod in InstantiatingMethods)
            {
                RemoveMethodFromCallableMethods(instantiatingMethod, method);
            }

            foreach(Method chainingMethod in ChainingMethods)
            {
                RemoveMethodFromCallableMethods(chainingMethod, method);
            }

            UpdateInterfaces();
        }

        #endregion

        #region Internal functions

        internal void UpdateInterfaces()
        {
            Interfaces.Clear();

            PopulateInterfacesForMethods(InstantiatingMethods);
            PopulateInterfacesForMethods(ChainingMethods);
        }

        #endregion

        #region Private functions

        private void SetDefaultFactoryClassName()
        {
            if(Name.HasText() && FactoryClassName.IsEmpty())
            {
                FactoryClassName = 
                    new CultureInfo(CultureInfo.CurrentCulture.Name, true).TextInfo.ToTitleCase(Name).Replace(" ", "") + "Builder";
            }
        }

        private void AddMethodToCollection(Method method)
        {
            switch (method.Group)
            {
                case Method.MethodGroup.Instantiating:
                    InstantiatingMethods.Add(method);
                    ChainStartingMethods.Add(method);
                    break;
                case Method.MethodGroup.Chaining:
                    ChainingMethods.Add(method);
                    ChainStartingMethods.Add(method);
                    ChainEndingMethods.Add(method);
                    break;
                case Method.MethodGroup.Executing:
                    ExecutingMethods.Add(method);
                    ChainEndingMethods.Add(method);
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.GroupIsNotValid);
            }
        }

        private void AddMethodAsCallableMethod(Method methodToAdd)
        {
            // Add this method as a callable method, to all ChainStarting methods,
            // if this is a ChainEnding method.
            if (methodToAdd.IsChainEnding)
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

        private void AddMethodToCallableMethods(Method method, Method callableMethod)
        {
            if (!method
                    .MethodsCallableNext
                    .Any(cm => cm.Group == callableMethod.Group.ToString() &&
                               cm.DatatypeSignature == callableMethod.DatatypeSignature))
            {
                method
                    .MethodsCallableNext
                    .Add(new CallableMethodIndicator(callableMethod));
            }
        }

        private void RemoveMethodFromCallableMethods(Method method, Method callableMethod)
        {
            CallableMethodIndicator callableMethodToRemove =
                method
                    .MethodsCallableNext
                    .FirstOrDefault(cm => cm.Group == callableMethod.Group.ToString() &&
                                          cm.Name == callableMethod.Name);

            if (callableMethodToRemove != null)
            {
                method.MethodsCallableNext.Remove(callableMethodToRemove);
            }
        }

        #endregion
    }
}