namespace Engine.Models
{
    public abstract class Datatype
    {
        public string Name { get; set; }
        public bool IsNative { get; set; }

        protected Datatype(string name, bool isNative)
        {
            Name = name;
            IsNative = isNative;
        }
    }
}