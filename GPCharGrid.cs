using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GPF_Editor
{
    public class GPCharGrid: INotifyPropertyChanged
    {
        private short _rowHeight;

        // Propery Changed Event
        public event PropertyChangedEventHandler? PropertyChanged;

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

        // OnPropertyChanged method (from INotifyPropertyChanged) to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
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
        protected void OnPropertyChanged([CallerMemberName] string name = null)
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

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
