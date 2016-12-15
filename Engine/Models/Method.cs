using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
    public class Method
    {
        public MethodAction ActionToPerform { get; }
        public string Name { get; }
        public int ChainIndex { get; }

        public ObservableCollection<ChainableMethod> ChainableMethods { get; }

        public ulong ChainMask
        {
            get
            {
                ulong mask = 0;

                foreach(ChainableMethod method in ChainableMethods.Where(x => x.IsSelected))
                {
                    mask += method.MaskValue;
                }

                return mask;
            }
        }

        public string SortKey => $"{ActionToPerform.ID}:{Name}";

        public Method(MethodAction actionToPerform, string name, int chainIndex)
        {
            ActionToPerform = actionToPerform;
            Name = name;
            ChainIndex = chainIndex;

            ChainableMethods = new ObservableCollection<ChainableMethod>();
        }

        public void AddChainableMethod(Method method)
        {
            if(ChainableMethods.All(x => x.Method.Name != method.Name))
            {
                ChainableMethods.Add(new ChainableMethod(method));
            }
        }
    }
}