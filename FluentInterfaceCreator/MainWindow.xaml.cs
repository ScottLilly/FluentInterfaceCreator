using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Engine.Models;
using Engine.Utilities;
using Engine.ViewModels;
using Microsoft.Win32;

namespace FluentInterfaceCreator
{
    public partial class MainWindow : Window
    {
        private const string FILE_NAME_EXTENSION = ".ficproj";

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
                    Filter = "Fluent Interface Creator projects (*.ficproj)|*.ficproj"
                };

            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                _projectEditor.LoadProjectFromXML(File.ReadAllText(dialog.FileName));

                _projectEditor.CurrentProject.FluentInterfaceFilesUpdated +=
                    CurrentProject_FluentInterfaceFilesUpdated;
            }
        }

        private void ApplicationSettings_OnClick(object sender, RoutedEventArgs e)
        {
            ApplicationSettings settings =
                new ApplicationSettings { Owner = this };

            settings.ShowDialog();
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

        private void SaveMethod_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.AddCurrentMethodToProject();
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

        private void CreateFluentInterface_OnClick(object sender, RoutedEventArgs e)
        {
            _projectEditor.CreateFluentInterface();
        }

        private void SaveProject_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog =
                new SaveFileDialog
                {
                    FileName = _projectEditor.CurrentProject.Name,
                    DefaultExt = FILE_NAME_EXTENSION,
                    Filter = "Fluent Interface Creator projects (*.ficproj)|*.ficproj"
                };

            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                File.WriteAllText(dialog.FileName,
                                  Serialization.Serialize(_projectEditor.CurrentProject));
            }
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
    }
}