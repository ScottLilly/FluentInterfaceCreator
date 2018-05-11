using System;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace FluentInterfaceCreator
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            VersionNumber.Content = Assembly.GetExecutingAssembly().GetName().Version;
            CopyrightNotice.Content = $"Copyright © 2016 - {DateTime.Now.Year}, Scott Lilly";
        }

        private void OnClick_OK(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SupportPage_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}