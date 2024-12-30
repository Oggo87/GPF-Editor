using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            get
            {
                return _rowHeight;
            }

            set
            {
                if (value != _rowHeight)
                {
                    _rowHeight = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<CharTableEntry> CharTable { get; set; }

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
    }

    public class CharTableEntry(char symbol, short width)
    {
        public char Symbol { get; set; } = symbol;
        public short Width { get; set; } = width;
    }
}
