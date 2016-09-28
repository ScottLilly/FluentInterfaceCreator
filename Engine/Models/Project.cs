using System.Collections.ObjectModel;

namespace Engine.Models
{
    public class Project
    {
        public string Name { get; private set; }
        public Language OutputLanguage { get; private set; }
        public ObservableCollection<Method> Methods { get; }
        public ObservableCollection<string> Datatypes { get; private set; }

        public Project(string name, Language outputLanguage)
        {
            Name = name;
            OutputLanguage = outputLanguage;

            Methods = new ObservableCollection<Method>();
            Datatypes = new ObservableCollection<string>(LanguageFactory.PrimitiveDatatypeFor(outputLanguage));
        }

        public void AddMethod(Method method)
        {
            // TODO: Throw exception if duplicate (by name, or by name and parameters?)
            Methods.Add(method);
        }
    }
}