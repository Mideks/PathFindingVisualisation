using CommunityToolkit.Mvvm.ComponentModel;
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
        public static readonly CellViewModel Unset = new();

        [ObservableProperty]
        private int x;
        [ObservableProperty]
        private int y;
        [ObservableProperty]
        private int size;

        [ObservableProperty]
        private CellState state;
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
