using System;
using System.Reflection;
using System.Windows;

namespace FluentInterfaceCreator
{
    public partial class About : Window
    {
        //private string VersionNumber { get; } =
        //    Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
    }
}