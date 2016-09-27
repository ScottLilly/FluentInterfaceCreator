namespace Engine.Models
{
    public class MethodParameter
    {
        public string Datatype { get; private set; }
        public string Name { get; private set; }

        public MethodParameter(string datatype, string name)
        {
            Datatype = datatype;
            Name = name;
        }
    }
}