using System.Linq;
using Engine.Models;
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
    }
}