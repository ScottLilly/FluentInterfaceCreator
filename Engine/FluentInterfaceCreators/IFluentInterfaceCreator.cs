using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    public interface IFluentInterfaceCreator
    {
        string CreateFluentInterfaceFor(Project project);
    }
}