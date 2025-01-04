using System.Diagnostics;
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
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            AuthorLabel.Content = versionInfo.CompanyName;
            CopyrightLabel.Content = versionInfo.LegalCopyright;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
