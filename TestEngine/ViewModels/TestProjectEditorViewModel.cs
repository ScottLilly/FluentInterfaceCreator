using Engine.Resources;
using Engine.ViewModels;
using Xunit;

namespace TestEngine.ViewModels
{
    public class TestProjectEditorViewModel
    {
        [Fact]
        public void Test_CreateFluentInterface()
        {
            ProjectEditorViewModel viewModel = new ProjectEditorViewModel();

            viewModel.CreateNewProject();
            viewModel.CurrentProject.Name = "Test fluent interface";

            Assert.Equal("TestFluentInterfaceBuilder", viewModel.CurrentProject.FactoryClassName);
        }

        [Fact]
        public void Test_HasProject()
        {
            ProjectEditorViewModel viewModel = new ProjectEditorViewModel();

            Assert.False(viewModel.HasProject);

            viewModel.CreateNewProject();

            Assert.True(viewModel.HasProject);
        }

        [Fact]
        public void Test_AddNewMethod_BlankValuesShowErrors()
        {
            ProjectEditorViewModel viewModel = new ProjectEditorViewModel();

            viewModel.CreateNewProject();
            viewModel.CurrentProject.Name = "Test fluent interface";

            // Missing Group, missing Name
            viewModel.CurrentEditingMethodGroup = null;

            viewModel.AddCurrentMethodToProject();

            Assert.Contains(ErrorMessages.GroupIsRequired,
                            viewModel.CurrentEditingMethodErrorMessage);
            Assert.Contains(ErrorMessages.NameIsRequired,
                            viewModel.CurrentEditingMethodErrorMessage);

            // Invalid Group
            //viewModel.CurrentEditingMethodGroup = "ASD";

            //viewModel.AddCurrentMethodToProject();

            //Assert.Contains(ErrorMessages.GroupIsNotValid,
            //                viewModel.CurrentEditingMethodErrorMessage);

            // Has valid Group, missing Name
            viewModel.CurrentEditingMethodGroup = "Instantiating";

            viewModel.AddCurrentMethodToProject();

            Assert.DoesNotContain(ErrorMessages.GroupIsRequired,
                                  viewModel.CurrentEditingMethodErrorMessage);
            Assert.Contains(ErrorMessages.NameIsRequired,
                            viewModel.CurrentEditingMethodErrorMessage);

            // Has valid Group and valid Name
            viewModel.CurrentEditingMethodName = "CreateEmail";

            viewModel.AddCurrentMethodToProject();

            Assert.DoesNotContain(ErrorMessages.GroupIsRequired,
                                  viewModel.CurrentEditingMethodErrorMessage);
            Assert.DoesNotContain(ErrorMessages.NameIsRequired,
                                  viewModel.CurrentEditingMethodErrorMessage);

            // Has valid Group, duplicate Name
            viewModel.CurrentEditingMethodName = "CreateEmail";

            viewModel.AddCurrentMethodToProject();

            Assert.DoesNotContain(ErrorMessages.GroupIsRequired,
                                  viewModel.CurrentEditingMethodErrorMessage);
            Assert.DoesNotContain(ErrorMessages.NameIsRequired,
                                  viewModel.CurrentEditingMethodErrorMessage);
            Assert.Contains(ErrorMessages.MethodAlreadyExists,
                            viewModel.CurrentEditingMethodErrorMessage);
        }
    }
}