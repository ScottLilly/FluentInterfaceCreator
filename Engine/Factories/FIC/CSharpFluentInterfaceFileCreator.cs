using System.Collections.Generic;
using System.Linq;
using Engine.Models;

namespace Engine.Factories.FIC
{
    internal sealed class CSharpFluentInterfaceFileCreator : 
        BaseFluentInterfaceFileCreator, IFluentInterfaceCreator
    {
        private enum InterfaceLocation
        {
            InBuilderFile,
            InSeparateFiles
        }

        public CSharpFluentInterfaceFileCreator(Project project) : base(project)
        {
        }

        #region Public functions

        public FluentInterfaceFile CreateInSingleFile()
        {
            return CreateBuilderFile(InterfaceLocation.InBuilderFile);
        }

        public IEnumerable<FluentInterfaceFile> CreateInMultipleFiles()
        {
            List<FluentInterfaceFile> files = new List<FluentInterfaceFile>();

            files.Add(CreateBuilderFile(InterfaceLocation.InSeparateFiles));
            files.AddRange(CreateInterfaceFiles(InterfaceLocation.InSeparateFiles));

            return files;
        }

        #endregion

        #region Private functions

        private FluentInterfaceFile CreateBuilderFile(InterfaceLocation interfaceLocation)
        {
            FluentInterfaceFile builder = 
                new FluentInterfaceFile($"{_project.FactoryClassName}.{_project.OutputLanguage.FileExtension}");

            // Find namespaces needed for "using" statements
            List<string> namespacesNeeded = new List<string>();

            foreach(Method method in _project.InstantiatingMethods)
            {
                namespacesNeeded.AddRange(method.NamespacesNeeded);
            }

            foreach(Method method in _project.ChainingMethods)
            {
                namespacesNeeded.AddRange(method.NamespacesNeeded);
            }

            foreach(Method method in _project.ExecutingMethods)
            {
                namespacesNeeded.AddRange(method.NamespacesNeeded);
            }

            // Add "using" statements
            foreach(string ns in namespacesNeeded.Distinct().OrderBy(n => n))
            {
                builder.AddLine(0, $"using {ns};");
            }

            if(namespacesNeeded.Any())
            {
                builder.AddBlankLine();
            }

            // Namespace definition
            builder.AddLine(0, $"namespace {_project.FactoryClassNamespace}");
            builder.AddLine(0, "{");

            // Class definition
            builder.AddLine(1, $"public class {_project.FactoryClassName} : {_project.InterfaceListAsCommaSeparatedString}");
            builder.AddLine(1, "{");

            // Instantiating functions
            builder.AddLine(2, "// Instantiating functions");

            foreach(Method method in _project.InstantiatingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    builder.AddBlankLine();
                    builder.AddLine(2, $"public static {returnDataType.Name} {method.Signature}");
                    builder.AddLine(2, "{");
                    builder.AddLine(3, $"return new {_project.FactoryClassName}();");
                    builder.AddLine(2, "}");
                }
            }

            // Chaining functions
            builder.AddBlankLine();
            builder.AddLine(2, "// Chaining functions");

            foreach(Method method in _project.ChainingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    builder.AddBlankLine();
                    builder.AddLine(2, $"public {returnDataType.Name} {method.Signature}");
                    builder.AddLine(2, "{");
                    builder.AddLine(3, "return this;");
                    builder.AddLine(2, "}");
                }
            }

            // Executing functions
            builder.AddBlankLine();
            builder.AddLine(2, "// Executing functions");

            foreach(Method method in _project.ExecutingMethods)
            {
                builder.AddBlankLine();
                builder.AddLine(2, $"public {method.ReturnDatatype.Name} {method.Signature}");
                builder.AddLine(2, "{");
                builder.AddLine(2, "}");
            }

            // Close class definition
            builder.AddLine(1, "}");

            // Append interfaces
            if(interfaceLocation == InterfaceLocation.InBuilderFile)
            {
                List<FluentInterfaceFile> interfaces =
                    CreateInterfaceFiles(InterfaceLocation.InBuilderFile);

                builder.AddBlankLine();
                builder.AddLine(1, "// Interfaces");

                foreach(FluentInterfaceFile interfaceFile in interfaces)
                {
                    builder.AddBlankLine();
                    builder.AddLine(0, interfaceFile.FormattedText());
                }
            }

            // Close namespace definition
            builder.AddLine(0, "}");

            return builder;
        }

        private List<FluentInterfaceFile> CreateInterfaceFiles(InterfaceLocation interfaceLocation)
        {
            List<FluentInterfaceFile> interfaces = new List<FluentInterfaceFile>();

            foreach(InterfaceData interfaceData in _project.Interfaces)
            {
                FluentInterfaceFile interfaceFile =
                    new FluentInterfaceFile($"{interfaceData.Name}.{_project.OutputLanguage.FileExtension}");

                if(interfaceLocation == InterfaceLocation.InSeparateFiles)
                {
                    // Find namespaces needed for "using" statements
                    List<string> namespacesNeeded = new List<string>();

                    foreach(Method method in interfaceData.CallableMethods)
                    {
                        namespacesNeeded.AddRange(method.NamespacesNeeded);
                    }

                    // Add "using" statements
                    foreach(string ns in namespacesNeeded.Distinct().OrderBy(n => n))
                    {
                        interfaceFile.AddLine(0, $"using {ns};");
                    }

                    if(namespacesNeeded.Any())
                    {
                        interfaceFile.AddBlankLine();
                    }

                    // Start namespace
                    interfaceFile.AddLine(0, $"namespace {_project.FactoryClassNamespace}");
                    interfaceFile.AddLine(0, "{");
                }

                interfaceFile.AddLine(1, $"public interface {interfaceData.Name}");

                interfaceFile.AddLine(1, "{");

                foreach(Method callableMethod in
                    interfaceData.CallableMethods
                                 .Where(cm => cm.Group == Method.MethodGroup.Instantiating ||
                                              cm.Group == Method.MethodGroup.Chaining))
                {
                    InterfaceData returnInterface =
                        _project.Interfaces
                                .FirstOrDefault(i => i.CalledByMethods.Exists(cm => cm.Name == callableMethod.Name));

                    interfaceFile.AddLine(2, $"{returnInterface?.Name} {callableMethod.Signature};");
                }

                foreach(Method callableMethod in
                    interfaceData.CallableMethods
                                 .Where(cm => cm.Group == Method.MethodGroup.Executing))
                {
                    interfaceFile.AddLine(2, $"{callableMethod.ReturnDatatype.Name} {callableMethod.Signature};");
                }

                interfaceFile.AddLine(1, "}");

                if(interfaceLocation == InterfaceLocation.InSeparateFiles)
                {
                    interfaceFile.AddLine(0, "}");
                }

                interfaces.Add(interfaceFile);
            }

            return interfaces;
        }

        private InterfaceData ReturnDataTypeForMethod(Method method)
        {
            return _project
                .Interfaces
                .FirstOrDefault(i => i.CalledByMethods.Exists(c => c.Name == method.Name));
        }

        #endregion
    }
}