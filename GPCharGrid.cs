using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GPF_Editor
{
    public class GPCharGrid: INotifyPropertyChanged
    {

        public Image? GridImage = null;

        // Propery Changed Event
        public event PropertyChangedEventHandler? PropertyChanged;

        private short _rowHeight;
        public short RowHeight
        {
            get => _rowHeight;

            set
            {
                if (value != _rowHeight)
                {
                    _rowHeight = value;
                    OnPropertyChanged();
                }
            }
        }
        public TrulyObservableCollection<CharTableEntry> CharTable { get; set; }

        public GPCharGrid()
        {
            CharTable = [];
        }

        public void UpdateGridImage(int size)
        {
            GridImage = new Image<Rgba32>(size, size);

            int currentRow = 0;

            int currentColumn = 0;

            PointF[] points;

            foreach (var entry in CharTable)
            {
                // Draw the first row
                points =
                [
                new PointF(0, currentRow),
                new PointF(size, currentRow),
                ];

                GridImage.Mutate(x => x.DrawLine(Pens.Solid(Color.Red, 1), points));

                if ((currentColumn += entry.Width) >= size)
                {
                    currentColumn = entry.Width;
                    currentRow += RowHeight;
                    
                    points =
                    [
                    new PointF(0, currentRow),
                    new PointF(size, currentRow),
                    ];

                    GridImage.Mutate(x => x.DrawLine(Pens.Solid(Color.Red, 1), points));

                }

                points =
                [
                new PointF(currentColumn, currentRow),
                new PointF(currentColumn, currentRow + RowHeight),
                ];

                GridImage.Mutate(x => x.DrawLine(Pens.Solid(Color.Red, 1), points));

            }

            // Draw the last row
            currentRow += RowHeight;

            points =
            [
                new PointF(0, currentRow),
                new PointF(size, currentRow),
            ];

            GridImage.Mutate(x => x.DrawLine(Pens.Solid(Color.Red, 1), points));


        }

        // OnPropertyChanged method (from INotifyPropertyChanged) to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Method to scale the grid
        public void ScaleGrid(float scale)
        {
            RowHeight = (short)(RowHeight * scale);
            foreach (var entry in CharTable)
            {
                entry.ScaleEntry(scale);
            }
        }
    }

    public class CharTableEntry: INotifyPropertyChanged
    {
        private short _width;
        private char _symbol;

        public char Symbol
        {
            get => _symbol;

            set
            {
                if (value != _symbol)
                {
                    _symbol = value;
                    OnPropertyChanged();
                }
            }
        }
        public short Width
        {
            get => _width;

            set
            {
                if (value != _width)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }

        public CharTableEntry()
        {
        }

        public CharTableEntry(char symbol, short width)
        {
            Symbol = symbol;
            Width = width;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ScaleEntry(float scale)
        {
            Width = (short)(Width * scale);
        }

        // OnPropertyChanged method (from INotifyPropertyChanged) to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public TrulyObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
