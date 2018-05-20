using System.Collections.Generic;
using Engine.Models;

namespace Engine.Factories
{
    public class FluentInterfaceCreator
    {
        private Project _project;

        public FluentInterfaceCreator(Project project)
        {
            _project = project;
        }

        #region Public functions

        public FluentInterfaceFile CreateInSingleFile()
        {
            return new FluentInterfaceFile("");
            //return CreateBuilderFile(ProjectEditorViewModel.InterfaceLocation.InBuilderFile);
        }

        public IEnumerable<FluentInterfaceFile> CreateInMultipleFiles()
        {
            List<FluentInterfaceFile> files = new List<FluentInterfaceFile>();

            //files.Add(CreateBuilderFile(ProjectEditorViewModel.InterfaceLocation.InSeparateFiles));
            //files.AddRange(CreateInterfaceFiles(ProjectEditorViewModel.InterfaceLocation.InSeparateFiles));

            return files;
        }

        #endregion
    }
}