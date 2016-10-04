namespace Engine.Models
{
    public class Method
    {
        public MethodAction ActionToPerform { get; private set; }
        public string Name { get; private set; }
        public string SortKey => $"{ActionToPerform.ID}:{Name}";

        public Method(MethodAction actionToPerform, string name)
        {
            ActionToPerform = actionToPerform;
            Name = name;
        }
    }
}