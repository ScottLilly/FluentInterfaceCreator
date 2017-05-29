using System.Windows;

namespace FluentInterfaceCreator
{
    public partial class ApplicationSettings : Window
    {
        public ApplicationSettings()
        {
            InitializeComponent();
        }

        private void OK_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}