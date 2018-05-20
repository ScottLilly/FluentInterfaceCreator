using Engine.Models;

namespace Engine.Factories.FIC
{
    public abstract class BaseFluentInterfaceFileCreator
    {
        protected readonly Project _project;

        protected BaseFluentInterfaceFileCreator(Project project)
        {
            _project = project;
        }
    }
}