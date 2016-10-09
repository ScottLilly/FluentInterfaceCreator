using System;
using System.Collections.ObjectModel;
using System.Linq;
using Engine.Resources;

namespace Engine.Models
{
    public class Project : BaseNotificationClass
    {
        private bool _isComplete;
        private bool _isDirty;

        public Project(string name, Language outputLanguage)
        {
            Name = name;
            OutputLanguage = outputLanguage;

            Methods = new ObservableCollection<Method>();

            IsDirty = false;
            IsComplete = false;
        }

        private int DetermineChainIndexFor(MethodAction methodAction)
        {
            if(methodAction == Actions.Instantiate)
            {
                return Constants.INITIATE_METHOD_ACTION_CHAIN_INDEX;
            }

            int chainIndex = 0;

            while(Methods.Any(x => x.ChainIndex == chainIndex))
            {
                chainIndex++;

                if(chainIndex == 64)
                {
                    throw new ArgumentOutOfRangeException(ErrorMessages.CannotAddMoreThan63MethodsToAProject);
                }
            }

            return chainIndex;
        }

        public void AddMethod(MethodAction methodAction, string name)
        {
            //TODO: Prevent duplicate method names.

            int chainIndex = DetermineChainIndexFor(methodAction);

            Method method = new Method(methodAction, name, chainIndex);

            Methods.Add(method);

            if(methodAction == Actions.Continue ||
                methodAction == Actions.Execute)
            {
                foreach(Method existingMethod in
                    Methods.Where(x =>
                    x.ActionToPerform == Actions.Instantiate ||
                    x.ActionToPerform == Actions.Continue))
                {
                    existingMethod.AddChainableMethod(method);
                }
            }

            IsDirty = true;
            IsComplete = false;
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            private set
            {
                _isComplete = value;

                NotifyPropertyChanged("IsComplete");
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                _isDirty = value;

                NotifyPropertyChanged("IsDirty");
            }
        }

        public ObservableCollection<Method> Methods { get; }

        public string Name { get; private set; }
        public Language OutputLanguage { get; private set; }
    }
}