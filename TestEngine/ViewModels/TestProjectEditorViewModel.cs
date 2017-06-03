using Engine.ViewModels;
using Xunit;

namespace TestEngine.ViewModels
{
    public class TestProjectEditorViewModel
    {
        [Fact]
        public void Test_CreateFluentInterface()
        {
            var viewModel = new ProjectEditorViewModel();

            viewModel.CreateNewProject();

            viewModel.CurrentProject.Name = "Test fluent interface";

            Assert.Equal("TestFluentInterfaceBuilder", viewModel.CurrentProject.FactoryClassName);
        }
    }
}