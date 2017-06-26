using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    internal class CSharpFluentInterfaceFileCreator : IFluentInterfaceCreator
    {
        private const string FILE_NAME_EXTENSION = ".cs";

        private Project _project;

        public FluentInterfaceFile CreateSingleFluentInterfaceFileFor(
            Project project)
        {
            _project = project;

            var interfaces = CreateInterfaceFiles();

            return CreateSingleFile(interfaces);
        }

        public List<FluentInterfaceFile> CreateSeparateFluentInterfaceFilesFor(
            Project project)
        {
            _project = project;

            List<FluentInterfaceFile> files = new List<FluentInterfaceFile>();

            files.Add(CreateSingleFile());
            files.AddRange(CreateInterfaceFiles(false));

            return files;
        }

        private FluentInterfaceFile CreateSingleFile(List<FluentInterfaceFile> interfaces = null)
        {
            StringBuilder builder = new StringBuilder();

            // Using statements


            // Namespace definition
            builder.AppendLine($"namespace {_project.FactoryClassName}");
            builder.AppendLine("{");

            // Class definition
            builder
                .AppendLine($"\tpublic class {_project.FactoryClassName} : {_project.InterfaceListAsCommaSeparatedString}");
            builder.AppendLine("\t{");

            // Instantiating functions
            builder.AppendLine("\t\t// Instantiating functions");

            foreach(Method method in _project.InstantiatingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    builder.AppendLine();
                    builder.AppendLine($"\t\tpublic static {returnDataType.Name} {method.Name}()");
                    builder.AppendLine("\t\t{");
                    builder.AppendLine($"\t\t\treturn new {_project.FactoryClassName}();");
                    builder.AppendLine("\t\t}");
                }
            }

            // Chaining functions
            builder.AppendLine();
            builder.AppendLine("\t\t// Chaining functions");

            foreach(Method method in _project.ChainingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    builder.AppendLine();
                    builder.AppendLine($"\t\tpublic {returnDataType.Name} {method.Name}()");
                    builder.AppendLine("\t\t{");
                    builder.AppendLine("\t\t\treturn this;");
                    builder.AppendLine("\t\t}");
                }
            }

            // Executing functions
            builder.AppendLine();
            builder.AppendLine("\t\t// Executing functions");

            foreach(Method method in _project.ExecutingMethods)
            {
                builder.AppendLine();
                builder.AppendLine($"\t\tpublic {method.ReturnDatatype} {method.Name}()");
                builder.AppendLine("\t\t{");
                builder.AppendLine("\t\t}");
            }

            // Close class definition
            builder.AppendLine("\t}");

            // Append interfaces
            if(interfaces != null)
            {
                builder.AppendLine("");
                builder.AppendLine("\t// Interfaces");
                builder.AppendLine("");

                foreach (FluentInterfaceFile interfaceFile in interfaces)
                {
                    builder.AppendLine(interfaceFile.Contents);
                }
            }

            // Close namespace definition
            builder.AppendLine("}");

            return new FluentInterfaceFile($"{_project.FactoryClassName}{FILE_NAME_EXTENSION}",
                                           builder.ToString());
        }

        private List<FluentInterfaceFile> CreateInterfaceFiles(bool includeTab = true)
        {
            List<FluentInterfaceFile> interfaces = new List<FluentInterfaceFile>();

            foreach(InterfaceData interfaceData in _project.Interfaces)
            {
                StringBuilder builder = new StringBuilder();

                if (includeTab)
                {
                    builder.Append("\t");
                }
                builder.AppendLine($"public interface {interfaceData.Name}");

                if(includeTab)
                {
                    builder.Append("\t");
                }
                builder.AppendLine("{");

                foreach(Method callableMethod in
                    interfaceData.CallableMethods
                                 .Where(cm => cm.Group == Method.MethodGroup.Instantiating ||
                                              cm.Group == Method.MethodGroup.Chaining))
                {
                    InterfaceData returnInterface =
                        _project.Interfaces
                                .FirstOrDefault(i => i.CalledByMethods.Exists(cm => cm.Name == callableMethod.Name));

                    if (includeTab)
                    {
                        builder.Append("\t");
                    }
                    builder.AppendLine($"\t{returnInterface?.Name} {callableMethod.Name}();");
                }

                foreach(Method callableMethod in
                    interfaceData.CallableMethods
                                 .Where(cm => cm.Group == Method.MethodGroup.Executing))
                {
                    if (includeTab)
                    {
                        builder.Append("\t");
                    }
                    builder.AppendLine($"\t{callableMethod.ReturnDatatype} {callableMethod.Name}();");
                }

                if (includeTab)
                {
                    builder.Append("\t");
                }
                builder.AppendLine("}");

                interfaces.Add(new FluentInterfaceFile($"{interfaceData.Name}{FILE_NAME_EXTENSION}",
                                                       builder.ToString()));
            }

            return interfaces;
        }

        private InterfaceData ReturnDataTypeForMethod(Method method)
        {
            return _project
                .Interfaces
                .FirstOrDefault(i => i.CalledByMethods.Exists(c => c.Name == method.Name));
        }
    }
}