using System.Collections.Generic;
using System.Linq;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories.FIC
{
    internal sealed class CSharpFluentInterfaceFileCreator : 
        BaseFluentInterfaceFileCreator, IFluentInterfaceCreator
    {
        private enum InterfaceLocation
        {
            BuilderFile,
            IndividualFiles
        }

        public CSharpFluentInterfaceFileCreator(Project project) : base(project)
        {
        }

        #region Public functions

        public FluentInterfaceFile CreateInSingleFile()
        {
            return CreateBuilderFile(InterfaceLocation.BuilderFile);
        }

        public IEnumerable<FluentInterfaceFile> CreateInMultipleFiles()
        {
            List<FluentInterfaceFile> files = new List<FluentInterfaceFile>();

            files.Add(CreateBuilderFile(InterfaceLocation.IndividualFiles));
            files.AddRange(CreateInterfaceFiles(InterfaceLocation.IndividualFiles));

            return files;
        }

        #endregion

        #region Private functions

        private FluentInterfaceFile CreateBuilderFile(InterfaceLocation interfaceLocation)
        {
            FluentInterfaceFile builder = 
                new FluentInterfaceFile($"{_project.FactoryClassName}.{_project.OutputLanguage.FileExtension}");

            AddRequiredUsingStatements(builder, _project.NamespacesNeeded());

            builder.AddLine(0, $"namespace {_project.FactoryClassNamespace}");
            builder.AddLine(0, "{");

            builder.AddLine(1, $"public class {_project.FactoryClassName} : {_project.InterfaceListAsCommaSeparatedString}");
            builder.AddLine(1, "{");

            AddInstantiatingFunctions(builder);
            AddChainingFunctions(builder);
            AddExecutingFunctions(builder);

            // Close class
            builder.AddLine(1, "}");

            // Append interfaces, if single file output
            if(interfaceLocation == InterfaceLocation.BuilderFile)
            {
                builder.AddLineAfterBlankLine(1, "// Interfaces");

                foreach(FluentInterfaceFile interfaceFile in 
                    CreateInterfaceFiles(InterfaceLocation.BuilderFile))
                {
                    builder.AddLineAfterBlankLine(0, interfaceFile.FormattedText());
                }
            }

            // Close namespace
            builder.AddLine(0, "}");

            return builder;
        }

        private List<FluentInterfaceFile> CreateInterfaceFiles(InterfaceLocation interfaceLocation)
        {
            List<FluentInterfaceFile> interfaces = new List<FluentInterfaceFile>();

            foreach(InterfaceData interfaceData in _project.Interfaces)
            {
                FluentInterfaceFile builder =
                    new FluentInterfaceFile($"{interfaceData.Name}.{_project.OutputLanguage.FileExtension}");

                if(interfaceLocation == InterfaceLocation.IndividualFiles)
                {
                    AddRequiredUsingStatements(builder, interfaceData.NamespacesNeeded());

                    // Start namespace
                    builder.AddLine(0, $"namespace {_project.FactoryClassNamespace}");
                    builder.AddLine(0, "{");
                }

                builder.AddLine(1, $"public interface {interfaceData.Name}");

                builder.AddLine(1, "{");

                foreach(Method callableMethod in
                    interfaceData.CallableMethods.Where(cm => cm.Group.IsChainStartingMethod()))
                {
                    InterfaceData returnInterface =
                        _project.Interfaces
                                .FirstOrDefault(i => i.CalledByMethods.Exists(cm => cm.Name == callableMethod.Name));

                    builder.AddLine(2, $"{returnInterface?.Name} {callableMethod.Signature};");
                }

                foreach(Method callableMethod in
                    interfaceData.CallableMethods
                                 .Where(cm => cm.Group == Method.MethodGroup.Executing))
                {
                    builder.AddLine(2, $"{callableMethod.ReturnDatatype.Name} {callableMethod.Signature};");
                }

                builder.AddLine(1, "}");

                if(interfaceLocation == InterfaceLocation.IndividualFiles)
                {
                    builder.AddLine(0, "}");
                }

                interfaces.Add(builder);
            }

            return interfaces;
        }

        private void AddInstantiatingFunctions(FluentInterfaceFile builder)
        {
            builder.AddLine(2, "// Instantiating functions");

            foreach(Method method in _project.InstantiatingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    builder.AddLineAfterBlankLine(2, $"public static {returnDataType.Name} {method.Signature}");
                    builder.AddLine(2, "{");
                    builder.AddLine(3, $"return new {_project.FactoryClassName}();");
                    builder.AddLine(2, "}");
                }
            }
        }

        private void AddChainingFunctions(FluentInterfaceFile builder)
        {
            builder.AddLineAfterBlankLine(2, "// Chaining functions");

            foreach(Method method in _project.ChainingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    builder.AddLineAfterBlankLine(2, $"public {returnDataType.Name} {method.Signature}");
                    builder.AddLine(2, "{");
                    builder.AddLine(3, "return this;");
                    builder.AddLine(2, "}");
                }
            }
        }

        private void AddExecutingFunctions(FluentInterfaceFile builder)
        {
            builder.AddLineAfterBlankLine(2, "// Executing functions");

            foreach(Method method in _project.ExecutingMethods)
            {
                builder.AddLineAfterBlankLine(2, $"public {method.ReturnDatatype.Name} {method.Signature}");
                builder.AddLine(2, "{");
                builder.AddLine(2, "}");
            }
        }

        private static void AddRequiredUsingStatements(FluentInterfaceFile builder, List<string> namespaces)
        {
            foreach(string ns in namespaces.Distinct().OrderBy(n => n))
            {
                builder.AddLine(0, $"using {ns};");
            }

            if(namespaces.Any())
            {
                builder.AddBlankLine();
            }
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