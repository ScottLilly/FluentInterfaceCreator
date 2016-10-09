using System;
using System.Linq;
using Engine;
using Engine.Models;
using Engine.ViewModels;
using Xunit;

namespace TestEngine.ViewModels
{
    public class TestProjectEditorViewModel
    {
        private readonly ProjectEditorViewModel _viewModel = new ProjectEditorViewModel();

        [Fact]
        public void Test_CreateFluentInterface()
        {
            _viewModel.CreateNewProject();

            // Add methods to fluent interface project
            AddMethodToProject(Actions.Instantiate, "CreateCalculator");
            Assert.Equal(1, _viewModel.CurrentProject.Methods.Count);

            AddMethodToProject(Actions.Continue, "InputNumber");
            Assert.Equal(2, _viewModel.CurrentProject.Methods.Count);

            AddMethodToProject(Actions.Execute, "CalculateMinimum");
            Assert.Equal(3, _viewModel.CurrentProject.Methods.Count);

            AddMethodToProject(Actions.Execute, "CalculateMean");
            Assert.Equal(4, _viewModel.CurrentProject.Methods.Count);

            AddMethodToProject(Actions.Execute, "CalculateMaximum");
            Assert.Equal(5, _viewModel.CurrentProject.Methods.Count);

            // Set selected method and confirm correct number of potentially-chainable methods.
            SetSelectedMethodTo("CreateCalculator");
            Assert.Equal(4, _viewModel.SelectedMethod.ChainableMethods.Count);
            Assert.Equal(Convert.ToUInt64(0), _viewModel.SelectedMethod.ChainMask);

            SetSelectedMethodTo("InputNumber");
            Assert.Equal(4, _viewModel.SelectedMethod.ChainableMethods.Count);
            Assert.Equal(Convert.ToUInt64(0), _viewModel.SelectedMethod.ChainMask);

            SetSelectedMethodTo("CalculateMinimum");
            Assert.Equal(0, _viewModel.SelectedMethod.ChainableMethods.Count);
            Assert.Equal(Convert.ToUInt64(0), _viewModel.SelectedMethod.ChainMask);

            SetSelectedMethodTo("CalculateMean");
            Assert.Equal(0, _viewModel.SelectedMethod.ChainableMethods.Count);
            Assert.Equal(Convert.ToUInt64(0), _viewModel.SelectedMethod.ChainMask);

            SetSelectedMethodTo("CalculateMaximum");
            Assert.Equal(0, _viewModel.SelectedMethod.ChainableMethods.Count);
            Assert.Equal(Convert.ToUInt64(0), _viewModel.SelectedMethod.ChainMask);
        }

        private void AddMethodToProject(MethodAction methodAction, string name)
        {
            _viewModel.MethodAction = methodAction;
            _viewModel.MethodName = name;
            _viewModel.AddMethod();
        }

        private void SetSelectedMethodTo(string name)
        {
            _viewModel.SelectedMethod =
                _viewModel.CurrentProject.Methods.FirstOrDefault(x => x.Name == name);
        }
    }
}