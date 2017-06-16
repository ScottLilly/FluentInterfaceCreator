using System.Collections.Generic;
using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    public interface IFluentInterfaceCreator
    {
        List<FluentInterfaceFile> CreateFluentInterfaceFilesFor(Project project, FileCreationOption fileCreationOption);
    }
}