using System.Collections.ObjectModel;

namespace Engine.Models
{
    public class Project
    {
        public string Name { get; private set; }
        public Language OutputLanguage { get; private set; }
        public bool IsDirty { get; private set; }

        public MethodList Methods { get; }
        public ObservableCollection<string> Datatypes { get; private set; }

        public Project(string name, Language outputLanguage)
        {
            Name = name;
            OutputLanguage = outputLanguage;
            IsDirty = false;

            Methods = new MethodList();
            Datatypes = new ObservableCollection<string>(LanguageFactory.PrimitiveDatatypesFor(outputLanguage));
        }

        public void AddMethod(Method method)
        {
            Methods.AddMethod(method);

            IsDirty = true;
        }
    }
}