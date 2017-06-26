using System.Collections.Generic;
using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    public interface IFluentInterfaceCreator
    {
        FluentInterfaceFile CreateSingleFluentInterfaceFileFor(Project project);
        List<FluentInterfaceFile> CreateSeparateFluentInterfaceFilesFor(Project project);
    }
}