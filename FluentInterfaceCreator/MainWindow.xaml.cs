using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine.Models;
using Engine.ViewModels;

namespace FluentInterfaceCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ProjectEditorViewModel _projectEditor = new ProjectEditorViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _projectEditor;
        }

        private void OnClick_CreateNewProject(object sender, RoutedEventArgs e)
        {
            _projectEditor.CreateNewProject();
        }

        private void OnClick_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClick_About(object sender, RoutedEventArgs e)
        {
            About about = new About {Owner = this};

            about.ShowDialog();
        }
    }
}
