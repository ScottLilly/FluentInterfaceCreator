using System.Collections.Generic;

namespace Engine.Models
{
    public class Project
    {
        public string Name { get; private set; }
        public List<Method> Methods { get; private set; }

        public Project(string name)
        {
            Name = name;
            Methods = new List<Method>();
        }
    }
}
