using PathFinding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Drawing;
using CommunityToolkit.Mvvm.Input;

namespace PathFindingVisualisation.ViewModel
{
    public partial class CellGridViewModel : ObservableObject
    {
        public ObservableCollection<CellViewModel> Cells { get; set; } = new();
        private HashSet<Location> walls = new();
        private Dictionary<Location, CellState> changedCellStates = new();
        private bool isDrawing =true;
        private CellState drawState;

        public CellGridViewModel()
        {
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    Cells.Add(new()
                    {
                        X = i,
                        Y = j,
                        State = CellState.Empty
                        //State = (CellState)(i % 6)
                        //TeteValue = $"Value{(i % 3)+ 1}"
                    });
                }
            }
        }

        public void Clear()
        {
            changedCellStates.Clear();
        }

        [RelayCommand]
        private void CellMouseMove(CellViewModel cell)
        {
            if (!isDrawing || cell == CellViewModel.Unset) return;
            var walkable = drawState != CellState.Wall;
            SetWalkable(cell, walkable);
        }

        private void SetWalkable(CellViewModel cell, bool walkable)
        {
            var location = new Location(cell.X, cell.Y);
            if (!changedCellStates.ContainsKey(location) && cell.State != CellState.Wall)
                changedCellStates[location] = cell.State;

            if (walkable)
            {
                // default is Empty

                var state = changedCellStates.GetValueOrDefault(location);
                cell.State = state;
                walls.Remove(location);
            }
            else
            {
                cell.State = CellState.Wall;
                walls.Add(location);
            }


        }

        [RelayCommand]
        private void CellMouseDown(CellViewModel cell)
        {
            isDrawing = true;
            drawState = cell.State == CellState.Wall ? CellState.Empty : CellState.Wall;
            CellMouseMove(cell);
        }

        [RelayCommand]
        private void CellMouseUp(CellViewModel cell)
        {
            isDrawing = false;
            drawState = CellState.Wall;
        }


        public void ChangeCellState(Location location, CellState state)
        {
            var cellViewModel = Cells.FirstOrDefault(c => c.X == location.X && c.Y == location.Y);
            if (cellViewModel != null)
            {
                cellViewModel.State = state;
            }
        }

        public List<Location> GetWalls()
        {
            return walls.ToList();
        }

    }
}
