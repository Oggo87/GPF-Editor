using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using IniParser.Model;
using IniParser;
using System.Text;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GPF_Editor
{
    public class GPFont : INotifyPropertyChanged
    {
        // Propery Changed Event
        public event PropertyChangedEventHandler? PropertyChanged;

        private Image<L8>? _fontImage;

        public Image<L8>? FontImage
        { 
            get => _fontImage;

            set
            {
                _fontImage = value;

                if(_fontImage != null)
                {
                    ScaleFactor = DefaultScaling * DefaultResolution / _fontImage.Width;
                    SaveEnabled = true;
                }
                else
                {
                    ScaleFactor = DefaultScaling;
                    SaveEnabled = false;
                }
            } 
        }
        public GPCharGrid CharGrid { get; set; }

        private int DefaultResolution { get; } = 256;

        private float DefaultScaling { get; } = 0.0015625f;

        private int ScalingAddress { get; } = 0x001fa0c8;

        public float ScaleFactor { get; private set; }

        private bool _saveEnabled;
        public bool SaveEnabled
        {
            get => _saveEnabled;
            private set
            {
                _saveEnabled = value;
                OnPropertyChanged();
            }
        }

        public GPFont()
        {
            CharGrid = new GPCharGrid();
            ScaleFactor = DefaultScaling;
        }

        // OnPropertyChanged method (from INotifyPropertyChanged) to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public WriteableBitmap? GetBMP()
        {

            WriteableBitmap? bmp = null;
                
            if(FontImage != null)
            {
                bmp = new WriteableBitmap(FontImage.Width, FontImage.Height, FontImage.Metadata.HorizontalResolution, FontImage.Metadata.VerticalResolution, PixelFormats.Bgra32, null);

                bmp.Lock();
                try
                {

                    FontImage.ProcessPixelRows(accessor =>
                    {
                        var backBuffer = bmp.BackBuffer;

                        for (var y = 0; y < FontImage.Height; y++)
                        {
                            Span<L8> pixelRow = accessor.GetRowSpan(y);

                            for (var x = 0; x < FontImage.Width; x++)
                            {
                                var backBufferPos = backBuffer + (y * FontImage.Width + x) * 4;
                                Rgba32 rgba = new();
                                rgba.FromL8(pixelRow[x]);
                                var color = rgba.A << 24 | rgba.R << 16 | rgba.G << 8 | rgba.B;

                                Marshal.WriteInt32(backBufferPos, color);
                            }
                        }
                    });

                    bmp.AddDirtyRect(new Int32Rect(0, 0, FontImage.Width, FontImage.Height));
                }
                finally
                {
                    bmp.Unlock();
                }
            }
            return bmp;
        }

        public void LoadGPF(Stream gpfStream)
        {
            using BinaryReader reader = new(gpfStream, System.Text.Encoding.Unicode);

            // Read number of CharGrid entries
            int numEntries = reader.ReadInt32();

            // Read CharGrid row height
            CharGrid.RowHeight = reader.ReadInt16();

            // Read TGA width and height
            int tgaWidth = reader.ReadInt32();
            int tgaHeight = reader.ReadInt32();

            // Clear CharGrid entries
            CharGrid.CharTable.Clear();

            // Read CharGrid entries
            for (int i = 0; i < numEntries; i++)
            {
                char symbol = reader.ReadChar();
                short width = reader.ReadInt16();
                CharGrid.CharTable.Add(new CharTableEntry(symbol, width));
            }

            // Read TGA image data
            byte[] tgaData = reader.ReadBytes(tgaWidth * tgaHeight);
            FontImage = Image.LoadPixelData<L8>(tgaData, tgaWidth, tgaHeight);
        }

        public bool SaveGPF(Stream gpfStream)
        {
            if(FontImage != null)
            {
                using BinaryWriter writer = new(gpfStream, System.Text.Encoding.Unicode);
                // Write number of CharGrid entries
                writer.Write(CharGrid.CharTable.Count);

                // Write CharGrid row height
                writer.Write(CharGrid.RowHeight);

                // Write TGA width and height
                writer.Write(FontImage.Width);
                writer.Write(FontImage.Height);

                // Write CharGrid entries
                foreach (var entry in CharGrid.CharTable)
                {
                    writer.Write(entry.Symbol);
                    writer.Write(entry.Width);
                }

                // Write TGA image data
                FontImage.ProcessPixelRows(accessor =>
                {
                    for (var y = 0; y < FontImage.Height; y++)
                    {
                        Span<L8> pixelRow = accessor.GetRowSpan(y);

                        Span<byte> byteSpan = MemoryMarshal.AsBytes(pixelRow);

                        writer.Write(byteSpan.ToArray());
                    }
                });

                return true;
            }

            return false;
        }

        public void ImportImage(Stream stream, bool autoScale = false)
        {
            ImportImage(Image.Load<L8>(stream), autoScale);
        }

        public void ImportImage(Image<L8> image, bool autoScale = false)
        { 
            int currentResolution = FontImage?.Width ?? DefaultResolution;

            if (autoScale)
            {
                float scaleFactor = image.Width / currentResolution;
                CharGrid.ScaleGrid(scaleFactor);
            }

            FontImage = image;
        }

        public void ExportTga(Stream stream)
        {
            FontImage.SaveAsTga(stream);
        }

        public void ScaleGrid(float scale)
        {
            CharGrid.ScaleGrid(scale);
        }

        public void ExportPatchFiles(string savePath)
        {
            ExportCapFile(savePath);
            ExportPatchFile(savePath);
            ExportTargetFile(savePath);
        }

        public void ExportCapFile(string savePath)
        {
            string capFile = Path.Combine(savePath, "fontscale.cap");
            FileStream capStream = new(capFile, FileMode.Create);
            using BinaryWriter capWriter = new(capStream);

            capWriter.Write("CBIN".ToCharArray());

            capWriter.Write(0);

            capWriter.Write(0);

            capWriter.Write(1); //number of entries in the cap file

            capWriter.Write((short)102); //GP4 v1.02

            capWriter.Write((short)1);

            capWriter.Write("%GP4EXE".ToCharArray());

            //zero fill
            for (var i = 0; i< 0x79; i++)
            {
                capWriter.Write((byte)0);
            }

            capWriter.Write(ScalingAddress);

            capWriter.Write(160); //address of the patch in the file

            capWriter.Write(Marshal.SizeOf(ScaleFactor));

            capWriter.Write(ScaleFactor);

        }

        public void ExportPatchFile(string savePath)
        {
            string patchFile = Path.Combine(savePath, "fontscale_patch.ini");

            byte[] scaleBytes = BitConverter.GetBytes(ScaleFactor);
            string scaleString = "0x" + string.Join(", 0x", scaleBytes.Select(b => b.ToString("X")));

            var parser = new FileIniDataParser();
            IniData data = new();

            data["Main"]["Format"] = "";
            data["V1.02"]["Offset1"] = "0x" + ScalingAddress.ToString("X8");
            data["V1.02"]["Code1"] = scaleString;

            parser.WriteFile(patchFile, data);
        }

        public void ExportTargetFile(string savePath)
        {
            string targetFile = Path.Combine(savePath, "fontscale_target.ini");

            byte[] scaleBytes = BitConverter.GetBytes(ScaleFactor);
            string scaleString = "0x" + string.Join(", 0x", scaleBytes.Select(b => b.ToString("X")));

            string texResString = FontImage != null ? FontImage.Width.ToString() : DefaultResolution.ToString();

            var parser = new FileIniDataParser();
            IniData data = new();

            data["offset1"]["name"] = texResString + " Fonts";
            data["offset1"]["description"] = texResString + "x" + texResString + " GPF Fonts";
            data["offset1"]["author"] = "GPF Font Editor";
            data["offset1"]["address"] = "0x" + ScalingAddress.ToString("X8");
            data["offset1"]["format"] = "float";
            data["offset1"]["valueinhex"] = scaleString;
            data["offset1"]["category"] = "";
            data["offset1"]["subcategory"] = "";
            data["offset1"]["skipoffset"] = "0";

            parser.WriteFile(targetFile, data);
        }
    }
}
