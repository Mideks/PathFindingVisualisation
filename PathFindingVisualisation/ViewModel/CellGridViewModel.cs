using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PathFinding;
using PathFindingVisualisation.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PathFindingVisualisation.ViewModel
{
    public class WalkableChangedEventArgs : EventArgs
    {
        public WalkableChangedEventArgs(Location location, bool isWalkable)
        {
            this.Location = location;
            this.IsWalkable = isWalkable;
        }

        public Location Location { get; }

        public bool IsWalkable { get; }
    }

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
            this.Cells.Where(c => c.State == CellState.Wall)
            .Select(c => c.Location);

        private readonly Dictionary<Location, CellState> dirtyCellStates = new();
        private bool isDrawing = true;

        private EditMode editMode;

        public ObservableCollection<CellViewModel> Cells { get; set; } = new();
        public Location Start
        {
            get
            {
                var cell = this.Cells.First(c => c.State == CellState.Start);
                return cell.Location;
            }
            set
            {
                var start = this.Start;

                if (!start.Equals(value))
                {
                    this.ChangeCellState(start, this.dirtyCellStates.GetValueOrDefault(start));
                    this.ChangeCellState(value, CellState.Start);
                    this.OnPropertyChanged(nameof(this.Start));
                }
            }
        }
        public Location Goal
        {
            get
            {
                var cell = this.Cells.First(c => c.State == CellState.Goal);
                return cell.Location;
            }
            set
            {
                var goal = this.Goal;
                if (!goal.Equals(value))
                {
                    this.ChangeCellState(goal, this.dirtyCellStates.GetValueOrDefault(goal));
                    this.ChangeCellState(value, CellState.Goal);
                    this.OnPropertyChanged(nameof(this.Goal));
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
                    this.Cells.Add(new(x, y) { State = CellState.Empty });

            this.ChangeCellState(start, CellState.Start);
            this.ChangeCellState(goal, CellState.Goal);
        }


        #region Methods
        public void ClearPath()
        {
            foreach (var cell in this.Cells)
            {
                if (cell.State == CellState.Visited ||
                    cell.State == CellState.Opened ||
                    cell.State == CellState.Path)
                    this.ChangeCellState(new Location(cell.X, cell.Y), CellState.Empty, false);
            }

            this.dirtyCellStates.Clear();
        }

        public void ClearWalls()
        {
            foreach (var cell in this.Cells)
            {
                if (cell.State == CellState.Wall)
                    cell.State = CellState.Empty;
            }
        }

        private void SetWalkable(CellViewModel cell, bool walkable)
        {
            var state = walkable ?
                this.dirtyCellStates.GetValueOrDefault(new Location(cell.X, cell.Y)) :
                CellState.Wall;

            this.ChangeCellState(cell, state);
            OnWalkableChangedEvent(new WalkableChangedEventArgs(cell.Location, walkable));
        }

        public void ChangeCellState(Location location, CellState state, bool canChangeStartAndGoal = true)
        {
            var cell = this.GetCell(location);
            if (cell != null)
            {
                this.ChangeCellState(cell, state, canChangeStartAndGoal);
            }
        }

        public CellViewModel? GetCell(Location location)
        {
            return this.Cells.FirstOrDefault(c => c.Location.Equals(location));
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
                this.dirtyCellStates[location] = cell.State;
            }
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void CellMouseMove(CellViewModel cell)
        {
            if (!this.isDrawing || cell == CellViewModel.Unset) return;

            if (this.editMode == EditMode.SetEmpty && cell.State != CellState.Start && cell.State != CellState.Goal)
            {
                this.SetWalkable(cell, true);
            }
            else if (this.editMode == EditMode.SetWall && cell.State != CellState.Start && cell.State != CellState.Goal)
            {
                this.SetWalkable(cell, false);
            }
            else if (this.editMode == EditMode.MoveStart && cell.State != CellState.Wall && cell.State != CellState.Goal)
            {
                this.Start = new Location(cell.X, cell.Y);
            }
            else if (this.editMode == EditMode.MoveGoal && cell.State != CellState.Wall && cell.State != CellState.Start)
            {
                this.Goal = new Location(cell.X, cell.Y);
            }
        }

        [RelayCommand]
        private void CellMouseDown(CellViewModel cell)
        {
            this.isDrawing = true;

            this.editMode = cell.State switch
            {
                CellState.Wall => EditMode.SetEmpty,
                CellState.Start => EditMode.MoveStart,
                CellState.Goal => EditMode.MoveGoal,
                _ => EditMode.SetWall
            };

            this.CellMouseMove(cell);
        }

        [RelayCommand]
        private void CellMouseUp(CellViewModel cell)
        {
            this.isDrawing = false;
            this.editMode = default;
        }
        #endregion

        #region Events
        public event EventHandler<WalkableChangedEventArgs>? WalkableChangedEvent;

        protected virtual void OnWalkableChangedEvent(WalkableChangedEventArgs e)
        {
            EventHandler<WalkableChangedEventArgs>? handler = WalkableChangedEvent;
            handler?.Invoke(this, e);
        }
        #endregion
    }
}
