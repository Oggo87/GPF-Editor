﻿using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace GPF_Editor
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
            VersionLabel.Content = Assembly.GetExecutingAssembly().GetName().Version;

            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            if (attributes.Length > 0)
            {
                AuthorLabel.Content = ((AssemblyCompanyAttribute)attributes[0]).Company;
            }

            attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            if (attributes.Length > 0)
            {
                CopyrightLabel.Content = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var ps = new ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}
