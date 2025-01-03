using Microsoft.Win32;
using System.Windows;

namespace GPF_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GPFEditor : Window
    {

        private static readonly GPFont GpFont = new();

        public GPFEditor()
        {
            InitializeComponent();

            DataContext = GpFont;

            CharTableDataGrid.ItemsSource = GpFont.CharGrid.CharTable;

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (version != null)
            {
                Title += " v" + version.Major + "." + version.Minor;
            }

        }

        //Force minimum size of the window
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            MinWidth = ActualWidth;
            MinHeight = ActualHeight;
            ClearValue(SizeToContentProperty);
        }

        private void BtnOpenTGA_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openTga = new()
            {
                Filter = "Targa Image|*.tga"
            };
            if (openTga.ShowDialog() == true)
            {
                using var tgaStream = openTga.OpenFile();

                bool autoScale = false;

                if (GpFont.FontImage != null)
                {
                    var autoScaleMsgBoxRslt = MessageBox.Show("Would you like to automatically scale the currently loaded character grid to fit the resolution of the new texture?", "Auto-scale", MessageBoxButton.YesNo);
                    autoScale = autoScaleMsgBoxRslt == MessageBoxResult.Yes;
                }

                GpFont.ImportImage(tgaStream, autoScale);
                GpfImage.Source = GpFont.GetBMP(ChkBoxShowGrid.IsChecked ?? false);
            }
        }

        private void BtnOpenGPF_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openGPF = new()
            {
                Filter = "GPF Font|*.gpf"
            };
            if (openGPF.ShowDialog() == true)
            {
                using var gpfStream = openGPF.OpenFile();
                GpFont.LoadGPF(gpfStream);
                GpfImage.Source = GpFont.GetBMP(ChkBoxShowGrid.IsChecked ?? false);
            }
        }

        private void BtnSaveTGA_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveTga = new()
            {
                Filter = "Targa Image|*.tga"
            };
            if (saveTga.ShowDialog() == true)
            {
                using var tgaStream = saveTga.OpenFile();
                GpFont.ExportTga(tgaStream);
            }
        }

        private void BtnSaveGPF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveGpf = new()
            {
                Filter = "GPF Font|*.gpf"
            };
            if (saveGpf.ShowDialog() == true)
            {
                using var gpfStream = saveGpf.OpenFile();
                GpFont.SaveGPF(gpfStream);
            }
        }

        private void BtnExportPatch_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savePatch = new()
            {
                Filter = "Directory | directory",
                FileName = "patch files"
            };
            if (savePatch.ShowDialog() == true)
            {
                string savePath = System.IO.Path.GetDirectoryName(savePatch.FileName) ?? "";
                GpFont.ExportPatchFiles(savePath);

            }
        }

        private void ChkBoxShowGrid_Changed(object sender, RoutedEventArgs e)
        {
            if(GpFont.FontImage != null)
            {
                GpfImage.Source = GpFont.GetBMP(ChkBoxShowGrid.IsChecked ?? false);
            }
        }
    }
}