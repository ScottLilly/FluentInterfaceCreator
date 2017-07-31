using System.Collections.Generic;
using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    internal abstract class FluentInterfaceFileCreatorBase
    {
        protected readonly Project _project;

        protected FluentInterfaceFileCreatorBase(Project project)
        {
            _project = project;
        }

        internal abstract FluentInterfaceFile CreateSingleFile();
        internal abstract IEnumerable<FluentInterfaceFile> CreateIndividualFiles();
    }
}