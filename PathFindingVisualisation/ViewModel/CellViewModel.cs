using CommunityToolkit.Mvvm.ComponentModel;
using PathFinding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingVisualisation.ViewModel
{
    public partial class CellViewModel : ObservableObject
    {
        public static readonly CellViewModel Unset = new(0, 0);

        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Location))]
        private int x;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Location))]
        private int y;
        [ObservableProperty]
        private int size;

        [ObservableProperty]
        private CellState state;

        
        public Location Location => new (X, Y);

        public CellViewModel(int x, int y)
        {
            
            this.x = x;
            this.y = y;
        }
    }

    public enum CellState
    {
        Empty,
        Wall,
        Start,
        Goal,
        Path,
        Visited,
        Opened,
    }
}
