using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    internal class CSharpSingleFileCreator : IFluentInterfaceCreator
    {
        private const string FILE_NAME_EXTENSION = ".cs";

        private Project _project;

        public List<FluentInterfaceFile> CreateFluentInterfaceFilesFor(
            Project project, FileCreationOption fileCreationOption)
        {
            _project = project;

            List<FluentInterfaceFile> files = new List<FluentInterfaceFile>();

            if(fileCreationOption == FileCreationOption.SingleFile)
            {
                files.Add(CreateSingleFile());
            }
            else if(fileCreationOption == FileCreationOption.MultipleFiles)
            {
                // TODO: Create a file for the factory, and for each interface
            }

            return files;
        }

        private FluentInterfaceFile CreateSingleFile()
        {
            StringBuilder singleFile = new StringBuilder();

            // Using statements


            // Namespace definition
            singleFile.AppendLine($"namespace {_project.FactoryClassName}");
            singleFile.AppendLine("{");

            // Class definition
            singleFile.AppendLine($"\tpublic class {_project.FactoryClassName} : {_project.InterfaceListAsCommaSeparatedString}");
            singleFile.AppendLine("\t{");

            // Instantiating functions
            singleFile.AppendLine("\t\t// Instantiating functions");

            foreach(Method method in _project.InstantiatingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    singleFile.AppendLine();
                    singleFile.AppendLine($"\t\tpublic static {returnDataType.Name} {method.Name}()");
                    singleFile.AppendLine("\t\t{");
                    singleFile.AppendLine($"\t\t\treturn new {_project.FactoryClassName}();");
                    singleFile.AppendLine("\t\t}");
                }
            }

            // Chaining functions
            singleFile.AppendLine();
            singleFile.AppendLine("\t\t// Chaining functions");

            foreach(Method method in _project.ChainingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    singleFile.AppendLine();
                    singleFile.AppendLine($"\t\tpublic {returnDataType.Name} {method.Name}()");
                    singleFile.AppendLine("\t\t{");
                    singleFile.AppendLine($"\t\t\treturn this;");
                    singleFile.AppendLine("\t\t}");
                }
            }

            // Executing functions
            singleFile.AppendLine();
            singleFile.AppendLine("\t\t// Executing functions");

            foreach(Method method in _project.ExecutingMethods)
            {
                singleFile.AppendLine();
                singleFile.AppendLine($"\t\tpublic string {method.Name}()");
                singleFile.AppendLine("\t\t{");
                singleFile.AppendLine("\t\t}");
            }

            // Close namespace definition
            singleFile.AppendLine("\t}");

            // Close class definition
            singleFile.AppendLine("}");

            return new FluentInterfaceFile($"{_project.FactoryClassName}.{FILE_NAME_EXTENSION}",
                                           singleFile.ToString());
        }

        private InterfaceData ReturnDataTypeForMethod(Method method)
        {
            return _project
                .Interfaces
                .FirstOrDefault(i => i.CalledByMethods.Exists(c => c.Name == method.Name));
        }
    }
}