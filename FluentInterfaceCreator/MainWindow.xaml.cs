using System.IO;
using System.Windows;
using System.Windows.Forms;
using Engine.Models;
using Engine.Utilities;
using Engine.ViewModels;
using FluentInterfaceCreator.Resources;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace FluentInterfaceCreator
{
    public partial class MainWindow : Window
    {
        private const string FILE_NAME_EXTENSION = ".ficp";

        private readonly ProjectEditorViewModel _projectEditor =
            new ProjectEditorViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _projectEditor;
        }

        #region "File" menu options

        private void CreateNewProject_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.CreateNewProject();

            _projectEditor.CurrentProject.FluentInterfaceFilesUpdated += 
                CurrentProject_FluentInterfaceFilesUpdated;
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
                _projectEditor.LoadProjectFromXML(File.ReadAllText(dialog.FileName));

                _projectEditor.CurrentProject.FluentInterfaceFilesUpdated +=
                    CurrentProject_FluentInterfaceFilesUpdated;
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
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

        private void AddNewDatatype_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.AddNewDatatype();
        }

        private void DeleteDatatype_OnClick(object sender, RoutedEventArgs e)
        {
            Datatype selectedDatatype = ((FrameworkElement)sender).DataContext as Datatype;

            _projectEditor.DeleteDatatype(selectedDatatype);
        }

        private void SaveMethodParameter_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.AddParameterToMethod();
        }

        private void DeleteMethodParameter_OnClick(object sender, RoutedEventArgs e)
        {
            Parameter selectedParameter = ((FrameworkElement)sender).DataContext as Parameter;

            _projectEditor.DeleteParameter(selectedParameter);
        }

        private void SaveMethod_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.AddNewMethod();
        }

        private void DeleteMethod_OnClick(object sender, RoutedEventArgs e)
        {
            Method selectedMethod = ((FrameworkElement)sender).DataContext as Method;

            _projectEditor.DeleteMethod(selectedMethod);
        }

        private void SelectMethodsCallableNext_OnClick(object sender, RoutedEventArgs e)
        {
            Method selectedMethod = ((FrameworkElement)sender).DataContext as Method;

            _projectEditor.SelectMethodsCallableNextFor(selectedMethod);
        }

        private void MethodCallableNext_OnChecked(object sender, RoutedEventArgs e)
        {
            _projectEditor.RefreshCurrentProjectInterfaces();
        }

        private void SelectInterfaceToName_OnClick(object sender, RoutedEventArgs e)
        {
            InterfaceData selectedInterface = ((FrameworkElement)sender).DataContext as InterfaceData;

            _projectEditor.CurrentInterface = selectedInterface;
        }

        private void SaveInterfaceName_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.SaveInterfaceName();
        }

        private void CreateFluentInterfaceFiles_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.CreateFluentInterface();
        }

        private void CurrentProject_FluentInterfaceFilesUpdated(object sender, System.EventArgs e)
        {
            if (_projectEditor.CurrentProject.HasSingleFluentInterfaceFile)
            {
                FluentInterfaceSingleFilesTabControl.SelectedIndex = 0;
            }

            if (_projectEditor.CurrentProject.HasSeparateFluentInterfaceFiles)
            {
                FluentInterfaceSeparateFilesTabControl.SelectedIndex = 0;
            }
        }

        private void SaveSingleBuilderFileToDisk_OnClick(object sender, RoutedEventArgs e)
        {
            // This requires adding a project reference to System.Windows.Forms.dll
            FolderBrowserDialog dialog =
                new FolderBrowserDialog
                {
                    Description = Literals.SaveSingleFile,
                    ShowNewFolderButton = true
                };

            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach(FluentInterfaceFile file in 
                    _projectEditor.CurrentProject.SingleFluentInterfaceFile)
                {
                    File.WriteAllText(Path.Combine(dialog.SelectedPath, file.FileName), file.Contents);
                }
            }
        }

        private void SaveSeparateFilesToDisk_OnClick(object sender, RoutedEventArgs e)
        {
            // This requires adding a project reference to System.Windows.Forms.dll
            FolderBrowserDialog dialog =
                new FolderBrowserDialog
                {
                    Description = Literals.SaveIndividualFiles,
                    ShowNewFolderButton = true
                };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (FluentInterfaceFile file in
                    _projectEditor.CurrentProject.SeparateFluentInterfaceFiles)
                {
                    File.WriteAllText(Path.Combine(dialog.SelectedPath, file.FileName), file.Contents);
                }
            }
        }

        private void SaveProject_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog =
                new SaveFileDialog
                {
                    FileName = _projectEditor.CurrentProject.Name,
                    DefaultExt = FILE_NAME_EXTENSION,
                    Filter = "Fluent Interface Creator projects (*.ficp)|*.ficp"
                };

            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                File.WriteAllText(dialog.FileName,
                                  Serialization.Serialize(_projectEditor.CurrentProject));
            }
        }
    }
}