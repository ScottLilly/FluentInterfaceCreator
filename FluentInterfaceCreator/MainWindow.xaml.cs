using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Engine.Models;
using Engine.Shared;
using Engine.ViewModels;
using FluentInterfaceCreator.Resources;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace FluentInterfaceCreator
{
    public partial class MainWindow : Window
    {
        private const string FILE_NAME_EXTENSION = ".ficp";

        private readonly ProjectEditorViewModel _projectEditorViewModel =
            new ProjectEditorViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _projectEditorViewModel;
        }

        #region "File" menu options

        // TODO: Move to ViewModel
        private void CreateNewProject_OnClick(object sender, RoutedEventArgs e)
        {
            string selectedLanguage = "C#"; // Currently, only support C#
            OutputLanguage outputLanguage = _projectEditorViewModel.OutputLanguages.First(ol => ol.Name == selectedLanguage);

            _projectEditorViewModel.StartNewProject(outputLanguage);
        }

        private void LoadProject_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog =
                new OpenFileDialog
                {
                    DefaultExt = FILE_NAME_EXTENSION,
                    Filter = "Fluent Interface Creator projects (*.ficp)|*.ficp"
                };

            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                _projectEditorViewModel.LoadProjectFromXML(File.ReadAllText(dialog.FileName));
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: Add Project.IsDirty and warn if there are unsaved changes
            Close();
        }

        #endregion

        #region "Help" menu options

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            About about = new About {Owner = this};

            about.ShowDialog();
        }

        #endregion

        #region Workspace button click handlers

        private void AddNewDatatype_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditorViewModel.AddNewDatatype();
        }

        private void DeleteDatatype_OnClick(object sender, RoutedEventArgs e)
        {
            Datatype selectedDatatype = ((FrameworkElement)sender).DataContext as Datatype;

            _projectEditorViewModel.DeleteDatatype(selectedDatatype);
        }

        private void AddMethodParameter_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditorViewModel.AddParameterToMethod();
        }

        private void DeleteMethodParameter_OnClick(object sender, RoutedEventArgs e)
        {
            Parameter selectedParameter = ((FrameworkElement)sender).DataContext as Parameter;

            _projectEditorViewModel.DeleteParameterFromMethod(selectedParameter);
        }

        private void AddMethod_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditorViewModel.AddNewMethod();
        }

        private void DeleteMethod_OnClick(object sender, RoutedEventArgs e)
        {
            Method selectedMethod = ((FrameworkElement)sender).DataContext as Method;

            _projectEditorViewModel.DeleteMethod(selectedMethod);
        }

        private void SelectMethodsCallableNext_OnClick(object sender, RoutedEventArgs e)
        {
            Method selectedMethod = ((FrameworkElement)sender).DataContext as Method;

            _projectEditorViewModel.SelectMethodsCallableNextFor(selectedMethod);
        }

        private void MethodCallableNext_OnChecked(object sender, RoutedEventArgs e)
        {
            _projectEditorViewModel.RefreshInterfaces();
        }

        private void SelectInterfaceToName_OnClick(object sender, RoutedEventArgs e)
        {
            InterfaceData selectedInterface = ((FrameworkElement)sender).DataContext as InterfaceData;

            _projectEditorViewModel.SelectedInterface = selectedInterface;
        }

        private void SaveInterfaceName_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditorViewModel.UpdateSelectedInterfaceName();
        }

        #endregion

        private void SaveFluentInterfaceInSingleFile_OnClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog =
                new FolderBrowserDialog
                {
                    Description = Literals.SaveFluentInterfaceInSingleFile,
                    ShowNewFolderButton = true
                };

            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FluentInterfaceFile fluentInterfaceFile = 
                    _projectEditorViewModel.FluentInterfaceInSingleFile();

                File.WriteAllText(Path.Combine(dialog.SelectedPath, fluentInterfaceFile.Name), fluentInterfaceFile.FormattedText());
            }
        }

        private void SaveFluentInterfaceInMultipleFiles_OnClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog =
                new FolderBrowserDialog
                {
                    Description = Literals.SaveFluentInterfaceInMultipleFiles,
                    ShowNewFolderButton = true
                };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IEnumerable<FluentInterfaceFile> fluentInterfaceFiles = 
                    _projectEditorViewModel.FluentInterfaceInMultipleFiles();

                foreach (FluentInterfaceFile file in fluentInterfaceFiles)
                {
                    File.WriteAllText(Path.Combine(dialog.SelectedPath, file.Name), file.FormattedText());
                }
            }
        }

        private void SaveProject_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog =
                new SaveFileDialog
                {
                    FileName = _projectEditorViewModel.CurrentProject.Name,
                    DefaultExt = FILE_NAME_EXTENSION,
                    Filter = "Fluent Interface Creator projects (*.ficp)|*.ficp"
                };

            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                File.WriteAllText(dialog.FileName,
                                  Serialization.Serialize(_projectEditorViewModel.CurrentProject));

                // TODO: Reset Project.IsDirty flag
            }
        }
    }
}