using System.Collections.ObjectModel;

namespace Engine.Models
{
    public class Method
    {
        public string Name { get; private set; }
        public string ActionToPerform { get; private set; }
        public ObservableCollection<MethodParameter> Parameters { get; private set; }

        public Method(string name, string actionToPerform)
        {
            Name = name;
            ActionToPerform = actionToPerform;

            Parameters = new ObservableCollection<MethodParameter>();
        }

        public void AddParameter(string name, string datatype)
        {
            Parameters.Add(new MethodParameter(name, datatype));
        }
    }
}