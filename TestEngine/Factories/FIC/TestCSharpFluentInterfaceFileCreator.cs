using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Engine.Factories;
using Engine.Factories.FIC;
using Engine.Models;
using Engine.ViewModels;
using Shouldly;
using Xunit;

namespace TestEngine.Factories.FIC
{
    public class TestCSharpFluentInterfaceFileCreator
    {
        private const string EXPECTED_OUTPUT_NAMESPACE =
            "TestEngine.Factories.FIC.ExpectedOutput.CSharp";

        [Fact]
        public void Test_CreateEmailBuilderFluentInterfaceFiles()
        {
            ProjectEditorViewModel vm = CreateCSharpProjectEditorViewModel();

            // Set project properties
            vm.CurrentProject.Name = "Email";
            vm.CurrentProject.FactoryClassNamespace = "Engine";

            // Add instantiating method(s)
            vm.MethodUnderEdit.Group = Method.MethodGroup.Instantiating;
            vm.MethodUnderEdit.Name = "CreateEmailFrom";
            vm.SetMethodParameterPropertiesTo("string", "emailAddress");
            vm.AddNewMethod();

            // Add chaining method(s)
            vm.MethodUnderEdit.Group = Method.MethodGroup.Chaining;
            vm.MethodUnderEdit.Name = "To";
            vm.SetMethodParameterPropertiesTo("string", "emailAddress");
            vm.AddNewMethod();

            vm.MethodUnderEdit.Group = Method.MethodGroup.Chaining;
            vm.MethodUnderEdit.Name = "Subject";
            vm.SetMethodParameterPropertiesTo("string", "subject");
            vm.AddNewMethod();

            vm.MethodUnderEdit.Group = Method.MethodGroup.Chaining;
            vm.MethodUnderEdit.Name = "Body";
            vm.SetMethodParameterPropertiesTo("string", "body");
            vm.AddNewMethod();

            // Add executing method(s)
            vm.MethodUnderEdit.Group = Method.MethodGroup.Executing;
            vm.MethodUnderEdit.Name = "Send";
            vm.MethodUnderEdit.ReturnDatatype = vm.DatatypeOf("void");
            vm.AddNewMethod();

            // Set callable methods
            vm.SetCallableMethods("CreateEmailFrom", "To");
            vm.SetCallableMethods("To", "To");
            vm.SetCallableMethods("To", "Subject");
            vm.SetCallableMethods("Subject", "Body");
            vm.SetCallableMethods("Body", "Send");

            vm.RefreshInterfaces();

            // Name interfaces
            vm.CurrentProject.Interfaces.Count.ShouldBe(4);

            vm.SetInterfaceName("Chaining:To", "ICanSetTo");
            vm.SetInterfaceName("Chaining:Subject|Chaining:To", "ICanSetToOrSubject");
            vm.SetInterfaceName("Chaining:Body", "ICanSetBody");
            vm.SetInterfaceName("Executing:Send", "ICanSend");

            // Perform tests
            vm.CurrentProject.OutputLanguage.Name.ShouldBe("C#");
            vm.CurrentProject.FactoryClassName.ShouldBe("EmailBuilder");

            vm.CurrentProject.InstantiatingMethods.Count.ShouldBe(1);
            vm.CurrentProject.ChainingMethods.Count.ShouldBe(3);
            vm.CurrentProject.ExecutingMethods.Count.ShouldBe(1);

            IFluentInterfaceCreator creator = 
                FluentInterfaceCreatorFactory.GetFluentInterfaceFileCreator(vm.CurrentProject);

            // Test single file matches expected output
            FluentInterfaceFile singleFile = creator.CreateInSingleFile();

            singleFile.Name.ShouldBe("EmailBuilder.cs");
            singleFile.FormattedText().ShouldBe(TextIn("SingleFile.EmailBuilder.txt"));


            // Test multiple file matches expected output
            IEnumerable<FluentInterfaceFile> multipleFiles = creator.CreateInMultipleFiles().ToList();

            multipleFiles.Count().ShouldBe(5);

            multipleFiles.First(x => x.Name == "EmailBuilder.cs")
                         .FormattedText().ShouldBe(TextIn("MultipleFiles.EmailBuilder.txt"));

            multipleFiles.First(x => x.Name == "ICanSetTo.cs")
                         .FormattedText().ShouldBe(TextIn("MultipleFiles.ICanSetTo.txt"));

            multipleFiles.First(x => x.Name == "ICanSetToOrSubject.cs")
                         .FormattedText().ShouldBe(TextIn("MultipleFiles.ICanSetToOrSubject.txt"));

            multipleFiles.First(x => x.Name == "ICanSetBody.cs")
                         .FormattedText().ShouldBe(TextIn("MultipleFiles.ICanSetBody.txt"));

            multipleFiles.First(x => x.Name == "ICanSend.cs")
                         .FormattedText().ShouldBe(TextIn("MultipleFiles.ICanSend.txt"));
        }

        private static ProjectEditorViewModel CreateCSharpProjectEditorViewModel()
        {
            ProjectEditorViewModel vm = new ProjectEditorViewModel();

            vm.StartNewProject(OutputLanguageFactory.GetLanguages().First(x => x.Name == "C#"));

            return vm;
        }

        private static string TextIn(string filename)
        {
            using(Stream stream = 
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream($"{EXPECTED_OUTPUT_NAMESPACE}.{filename}"))
            {
                using(StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    public static class HelperExtensions
    {
        public static Datatype DatatypeOf(this ProjectEditorViewModel vm, string datatype)
        {
            return vm.CurrentProject.Datatypes.First(x => x.Name.Equals(datatype, StringComparison.CurrentCultureIgnoreCase));
        }

        public static void SetMethodParameterPropertiesTo(this ProjectEditorViewModel vm, string datatype, string name)
        {
            vm.ParameterUnderEdit.Datatype = vm.DatatypeOf(datatype);
            vm.ParameterUnderEdit.Name = name;
            vm.AddParameterToMethod();
        }

        public static void SetCallableMethods(this ProjectEditorViewModel vm, string chainStartingMethodName,
                                              params string[] callableMethods)
        {
            foreach(CallableMethodIndicator method in vm.CurrentProject.ChainStartingMethods
                                                        .First(x => x.Name == chainStartingMethodName)
                                                        .MethodsCallableNext)
            {
                if(callableMethods.Contains(method.Name))
                {
                    method.CanCall = true;
                }
            }
        }

        public static void SetInterfaceName(this ProjectEditorViewModel vm, string callableMethodSignature,
                                            string interfaceName)
        {
            vm.CurrentProject.Interfaces
              .First(x => x.CallableMethodsSignature == callableMethodSignature)
              .Name = interfaceName;
        }
    }
}
