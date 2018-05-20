using System.Linq;
using Engine.Models;
using Engine.Resources;
using Engine.ViewModels;
using Shouldly;
using Xunit;

namespace TestEngine.ViewModels
{
    public class TestProjectEditorViewModel
    {
        [Fact]
        public void Test_CreateNewViewModel()
        {
            ProjectEditorViewModel vm = new ProjectEditorViewModel();

            // Assertions
            vm.OutputLanguages.Count.ShouldBe(1);
            vm.OutputLanguages.Exists(ol => ol.Name == "C#").ShouldBeTrue();

            vm.CurrentProject.ShouldBeNull();
        }

        [Fact]
        public void Test_StartNewProject()
        {
            ProjectEditorViewModel vm = new ProjectEditorViewModel();

            OutputLanguage outputLanguage = vm.OutputLanguages.First(ol => ol.Name == "C#");

            vm.StartNewProject(outputLanguage);

            // Assertions
            vm.CurrentProject.ShouldNotBeNull();
            vm.CurrentProject.Datatypes.Count.ShouldBe(17);
        }

        [Fact]
        public void Test_AddNewDatatype_InputValidation()
        {
            ProjectEditorViewModel vm = new ProjectEditorViewModel();

            OutputLanguage outputLanguage = vm.OutputLanguages.First(ol => ol.Name == "C#");

            vm.StartNewProject(outputLanguage);

            vm.DatatypeUnderEdit.Name = "";
            vm.DatatypeUnderEdit.ContainingNamespace = "";
            vm.AddNewDatatype();

            // Datatype.Name is empty
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeFalse();

            // Datatype.Name contains internal space
            vm.DatatypeUnderEdit.Name = "asd asd";
            vm.DatatypeUnderEdit.ContainingNamespace = "";
            vm.AddNewDatatype();

            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeFalse();

            // Datatype.Name contains internal space AND special character
            vm.DatatypeUnderEdit.Name = "asd asd#";
            vm.DatatypeUnderEdit.ContainingNamespace = "";
            vm.AddNewDatatype();

            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeFalse();

            // Datatype.Name contains special character
            vm.DatatypeUnderEdit.Name = "asdasd#";
            vm.DatatypeUnderEdit.ContainingNamespace = "";
            vm.AddNewDatatype();

            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeFalse();

            // Datatype.ContainingName contains internal space and special character
            vm.DatatypeUnderEdit.Name = "asdasd";
            vm.DatatypeUnderEdit.ContainingNamespace = "asd asd#";
            vm.AddNewDatatype();

            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeTrue();

            // Datatype.ContainingName contains internal space
            vm.DatatypeUnderEdit.Name = "asdasd";
            vm.DatatypeUnderEdit.ContainingNamespace = "asd asd";
            vm.AddNewDatatype();

            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeTrue();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeFalse();

            // Datatype.ContainingName contains special character
            vm.DatatypeUnderEdit.Name = "asdasd";
            vm.DatatypeUnderEdit.ContainingNamespace = "asdasd#";
            vm.AddNewDatatype();

            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            vm.DatatypeUnderEditErrorMessage.Contains(ErrorMessages.NamespaceCannotContainSpecialCharacters).ShouldBeTrue();

        }
    }
}