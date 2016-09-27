namespace Engine.Models
{
    public class Method
    {
        public enum Action
        {
            Instantiate,
            Continue,
            Execute
        }

        public string Name { get; private set; }
        public Action ActionToPerform { get; private set; }

        public Method(string name, Action actionToPerform)
        {
            Name = name;
            ActionToPerform = actionToPerform;
        }
    }
}
