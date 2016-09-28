namespace Engine.Models
{
    public class MethodParameter
    {
        public string Datatype { get; private set; }
        public string Name { get; private set; }

        public MethodParameter(string name, string datatype)
        {
            Datatype = datatype;
            Name = name;
        }
    }
}