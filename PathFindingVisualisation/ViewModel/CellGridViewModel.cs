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

namespace PathFindingVisualization.ViewModel
{
    public partial class CellGridViewModel : ObservableObject
    {
        #region Varribles
        private enum EditMode
        {
            None,
            SetEmpty,
            SetWall,
            MoveStart,
            MoveGoal
        }

        public IEnumerable<Location> Walls => 
            Cells.Where(c => c.State == CellState.Wall)
            .Select(c => new Location(c.X, c.Y));

        private readonly Dictionary<Location, CellState> dirtyCellStates = new();
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
                var start = Start;
                if (!start.Equals(value))
                {
                    ChangeCellState(start, dirtyCellStates.GetValueOrDefault(start));
                    ChangeCellState(value, CellState.Start);
                    OnPropertyChanged(nameof(Start));
                }
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
                var goal = Goal;
                if(!goal.Equals(value))
                {
                    ChangeCellState(goal, dirtyCellStates.GetValueOrDefault(goal));
                    ChangeCellState(value, CellState.Goal);
                    OnPropertyChanged(nameof(Goal));
                }
            }
        }

        public int Width { get; }
        public int Height { get; }

        #endregion

        public CellGridViewModel(Location start, Location goal, int width, int height)
        {
            this.Width = width;
            this.Height = height;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Cells.Add(new(x, y) { State = CellState.Empty});

            ChangeCellState(start, CellState.Start);
            ChangeCellState(goal, CellState.Goal);
        }


        #region Methods
        public void ClearPath()
        {
            foreach (var cell in Cells)
            {
                if (cell.State == CellState.Visited ||
                    cell.State == CellState.Opened ||
                    cell.State == CellState.Path)
                    ChangeCellState(new Location(cell.X, cell.Y), CellState.Empty, false);
            }

            dirtyCellStates.Clear();
        }

        public void ClearWalls()
        {
            foreach (var cell in Cells)
            {
                if (cell.State == CellState.Wall) 
                    cell.State = CellState.Empty;
            }
        }

        private void SetWalkable(CellViewModel cell, bool walkable)
        {
            var state = walkable ? 
                dirtyCellStates.GetValueOrDefault(new Location(cell.X, cell.Y)) :
                CellState.Wall;

            ChangeCellState(cell, state);
        }

        public void ChangeCellState(Location location, CellState state, bool canChangeStartAndGoal = true)
        {
            var cell = GetCell(location);
            if (cell != null)
            {
                ChangeCellState(cell, state, canChangeStartAndGoal);
            }
        }

        public CellViewModel? GetCell(Location location)
        {
            return Cells.FirstOrDefault(c => c.Location.Equals(location));
        }

        private void ChangeCellState(CellViewModel cell, CellState state, bool canChangeStartAndGoal = true)
        {
            if (!canChangeStartAndGoal && (cell.State == CellState.Start || cell.State == CellState.Goal)) return;

            cell.State = state;
            var location = new Location(cell.X, cell.Y);

            // Помечаем "загрязнённые" клетки
            if (state == CellState.Visited ||
                state == CellState.Opened ||
                state == CellState.Path)
            {
                dirtyCellStates[location] = cell.State;
            }
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void CellMouseMove(CellViewModel cell)
        {
            if (!isDrawing || cell == CellViewModel.Unset) return;

            if (editMode == EditMode.SetEmpty && cell.State != CellState.Start && cell.State != CellState.Goal)
            {
                SetWalkable(cell, true);
            }
            else if (editMode == EditMode.SetWall && cell.State != CellState.Start && cell.State != CellState.Goal)
            {
                SetWalkable(cell, false);
            }
            else if (editMode == EditMode.MoveStart && cell.State != CellState.Wall && cell.State != CellState.Goal)
            {
                Start = new Location(cell.X, cell.Y);
            }
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
