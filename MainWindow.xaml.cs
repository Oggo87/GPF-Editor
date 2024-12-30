using Microsoft.Win32;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GPF_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        GPFont gpFont = new();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = gpFont;

            charTableDataGrid.ItemsSource = gpFont.CharGrid.CharTable;

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
                gpFont.FontImage = SixLabors.ImageSharp.Image.Load<L8>(tgaStream);
                gpfImage.Source = gpFont.GetBMP();
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
                gpFont.LoadGPF(gpfStream);
                gpfImage.Source = gpFont.GetBMP();
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
                gpFont.FontImage.SaveAsTga(tgaStream);
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
                gpFont.SaveGPF(gpfStream);
            }
        }
    }
}