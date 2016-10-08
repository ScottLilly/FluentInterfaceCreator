using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
    public class Method
    {
        public MethodAction ActionToPerform { get; private set; }
        public string Name { get; private set; }

        public ObservableCollection<SelectableMethod> ChainableMethods { get; }

        public string SortKey => $"{ActionToPerform.ID}:{Name}";

        public Method(MethodAction actionToPerform, string name)
        {
            ActionToPerform = actionToPerform;
            Name = name;

            ChainableMethods = new ObservableCollection<SelectableMethod>();
        }

        public void AddChainableMethods(Method method)
        {
            // Execute methods are "final" methods,
            // and do not have any methods that can be called after them.
            if(method.Name != Name &&
               (ActionToPerform == Actions.Instantiate ||
                ActionToPerform == Actions.Continue))
            {
                if(ChainableMethods.All(x => x.Method.Name != method.Name))
                {
                    ChainableMethods.Add(new SelectableMethod(method));
                }
            }
        }
    }
}