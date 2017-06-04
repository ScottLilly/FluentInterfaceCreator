using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

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

        public ObservableCollection<Method> InstantiatingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ChainingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<Method> ExecutingMethods { get; set; } =
            new ObservableCollection<Method>();

        public ObservableCollection<InterfaceData> Interfaces { get; set; } =
            new ObservableCollection<InterfaceData>();

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
            switch(methodToAdd.Group)
            {
                case Method.MethodGroup.Instantiating:
                    InstantiatingMethods.Add(methodToAdd);
                    break;
                case Method.MethodGroup.Chaining:
                    ChainingMethods.Add(methodToAdd);
                    break;
                case Method.MethodGroup.Executing:
                    ExecutingMethods.Add(methodToAdd);
                    break;
                default:
                    throw new ArgumentException();
            }

            SetDirty();

            foreach(Method instantiatingMethod in InstantiatingMethods)
            {
                AddMethodToCallableMethods(instantiatingMethod, methodToAdd);
            }

            foreach(Method chainingMethod in ChainingMethods)
            {
                AddMethodToCallableMethods(chainingMethod, methodToAdd);
            }

            NotifyPropertyChanged(nameof(ChainStartingMethods));
            NotifyPropertyChanged(nameof(ChainEndingMethods));
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
                    throw new ArgumentException();
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