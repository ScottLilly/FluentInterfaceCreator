using System.Linq;
using System.Text;
using Engine.Models;

namespace Engine.FluentInterfaceCreators
{
    internal class CSharpSingleFileCreator : IFluentInterfaceCreator
    {
        private Project _project;

        public string CreateFluentInterfaceFor(Project project)
        {
            _project = project;

            StringBuilder singleFile = new StringBuilder();

            // Using statements


            // Namespace definition
            singleFile.AppendLine($"namespace {project.FactoryClassName}");
            singleFile.AppendLine("{");

            // Class definition
            singleFile.AppendLine($"\tpublic class {project.FactoryClassName}");
            singleFile.AppendLine("\t{");

            // Instantiating functions
            singleFile.AppendLine("#region Instantiating functions");

            foreach(Method method in project.InstantiatingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if(returnDataType != null)
                {
                    singleFile.AppendLine();
                    singleFile.AppendLine($"\t\tpublic static {returnDataType.Name} {method.Name}()");
                    singleFile.AppendLine("\t\t{");
                    singleFile.AppendLine("\t\t}");
                }
            }

            singleFile.AppendLine("#endregion");

            // Chaining functions
            singleFile.AppendLine();
            singleFile.AppendLine("#region Chaining functions");

            foreach(Method method in project.ChainingMethods)
            {
                InterfaceData returnDataType = ReturnDataTypeForMethod(method);

                if (returnDataType != null)
                {
                    singleFile.AppendLine();
                    singleFile.AppendLine($"\t\tpublic {returnDataType.Name} {method.Name}()");
                    singleFile.AppendLine("\t\t{");
                    singleFile.AppendLine("\t\t}");
                }
            }

            singleFile.AppendLine();
            singleFile.AppendLine("#endregion");

            // Executing functions
            singleFile.AppendLine();
            singleFile.AppendLine("#region Executing functions");

            foreach(Method method in project.ExecutingMethods)
            {
                singleFile.AppendLine();
                singleFile.AppendLine($"\t\tpublic string {method.Name}()");
                singleFile.AppendLine("\t\t{");
                singleFile.AppendLine("\t\t}");
            }

            singleFile.AppendLine();
            singleFile.AppendLine("#endregion");


            // Close namespace definition
            singleFile.AppendLine("\t}");

            // Close class definition
            singleFile.AppendLine("}");

            return singleFile.ToString();
        }

        private InterfaceData ReturnDataTypeForMethod(Method method)
        {
            return _project.Interfaces
                           .FirstOrDefault(i => i.CalledByMethods.Exists(c => c.Name == method.Name));
        }
    }
}