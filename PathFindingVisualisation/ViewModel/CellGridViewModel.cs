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
        private enum EditMode
        {
            None,
            SetEmpty,
            SetWall,
            MoveStart,
            MoveGoal
        }

        private HashSet<Location> walls = new();
        private Dictionary<Location, CellState> changedCellStates = new();
        private bool isDrawing = true;

        private EditMode editMode;

        public ObservableCollection<CellViewModel> Cells { get; set; } = new();
        public Location Start
        {
            get 
            {
                var cell = Cells.First(c => c.State == CellState.Start);
                return new Location(cell.X, cell.Y);
            }
            set
            {
                ChangeCellState(Start, changedCellStates.GetValueOrDefault(Start));
                ChangeCellState(value, CellState.Start);
            }
        } 
        public Location Goal
        {
            get 
            {
                var cell = Cells.First(c => c.State == CellState.Goal);
                return new Location(cell.X, cell.Y);
            }
            set
            {
                ChangeCellState(Goal, changedCellStates.GetValueOrDefault(Goal));
                ChangeCellState(value, CellState.Goal);
            }
        }

        public CellGridViewModel(Location start, Location goal)
        {
            ChangeCellState(start, CellState.Start);
            ChangeCellState(start, CellState.Goal);

            // todo: возможность задавать размер динамически
            var width = 25;
            var height = 25;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cells.Add(new()
                    {
                        X = x, 
                        Y = y,
                        State = CellState.Empty
                    });
                }
            }
        }


        #region Methods
        public void Clear()
        {
            changedCellStates.Clear();
        }

        private void SetWalkable(CellViewModel cell, bool walkable)
        {
            var location = new Location(cell.X, cell.Y);

            if (walkable)
            {
                // default is Empty
                var state = changedCellStates.GetValueOrDefault(location);
                ChangeCellState(cell, state);
                walls.Remove(location);
            }
            else
            {
                ChangeCellState(cell, CellState.Wall);
                walls.Add(location);
            }
        }

        public void ChangeCellState(Location location, CellState state)
        {
            var cellViewModel = Cells.FirstOrDefault(c => c.X == location.X && c.Y == location.Y);
            if (cellViewModel != null)
            {
                ChangeCellState(cellViewModel, state);
            }
        }

        private void ChangeCellState(CellViewModel cell, CellState state)
        {
            cell.State = state;
            var location = new Location(cell.X, cell.Y);
            // Не считаем за изменения установку стен, смену начальной точки и конечной.
            if (!changedCellStates.ContainsKey(location) &&
                cell.State != CellState.Wall &&
                cell.State != CellState.Start &&
                cell.State != CellState.Goal)
            {
                changedCellStates[location] = cell.State;
            }
        }

        public List<Location> GetWalls()
        {
            return walls.ToList();
        }

        #endregion

        #region Commands
        [RelayCommand]
        private void CellMouseMove(CellViewModel cell)
        {
            if (!isDrawing || cell == CellViewModel.Unset) return;

            if (editMode == EditMode.SetEmpty)
            {
                SetWalkable(cell, true);

            }
            else if (editMode == EditMode.SetWall)
            {
                SetWalkable(cell, false);
            }
            // не должно заменять стены и конечную точку
            else if (editMode == EditMode.MoveStart && cell.State != CellState.Wall && cell.State != CellState.Goal)
            {
                Start = new Location(cell.X, cell.Y);
            }
            // не должно заменять стены и начальную точку
            else if (editMode == EditMode.MoveGoal && cell.State != CellState.Wall && cell.State != CellState.Start)
            {
                Goal = new Location(cell.X, cell.Y);
            }
        }

        [RelayCommand]
        private void CellMouseDown(CellViewModel cell)
        {
            isDrawing = true;

            editMode = cell.State switch
            {
                CellState.Wall => EditMode.SetEmpty,
                CellState.Start => EditMode.MoveStart,
                CellState.Goal => EditMode.MoveGoal,
                _ => EditMode.SetWall
            };

            CellMouseMove(cell);
        }

        [RelayCommand]
        private void CellMouseUp(CellViewModel cell)
        {
            isDrawing = false;
            editMode = default;
        }
        #endregion
    }
}
