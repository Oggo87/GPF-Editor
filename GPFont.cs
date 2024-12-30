using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using SixLabors.ImageSharp.ColorSpaces;
using System.Runtime.InteropServices;

namespace GPF_Editor
{
    public class GPFont
    {
        public Image<L8> FontImage { get; set; }
        public GPCharGrid CharGrid { get; set; }

        public GPFont()
        {
            CharGrid = new GPCharGrid();
        }

        public WriteableBitmap GetBMP()
        {
            var bmp = new WriteableBitmap(FontImage.Width, FontImage.Height, FontImage.Metadata.HorizontalResolution, FontImage.Metadata.VerticalResolution, PixelFormats.Bgra32, null);

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

                            System.Runtime.InteropServices.Marshal.WriteInt32(backBufferPos, color);
                        }
                    }
                });

                bmp.AddDirtyRect(new Int32Rect(0, 0, FontImage.Width, FontImage.Height));
            }
            finally
            {
                bmp.Unlock();
            }
            return bmp;
        }

        internal void LoadGPF(Stream gpfStream)
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

        internal void SaveGPF(Stream gpfStream)
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
        }
    }
}
