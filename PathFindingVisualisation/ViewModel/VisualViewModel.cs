using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PathFinding;
using PathFinding.Graphs;
using PathFinding.Searchers;
using PathFindingVisualisation.Enums;
using PathFindingVisualisation.Model;
using PathFindingVisualisation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PathFindingVisualisation.ViewModel
{
    public partial class VisualViewModel : ObservableObject
    {

        [ObservableProperty]
        private CellGridViewModel cellGrid;
        [ObservableProperty]
        private PointCollection path = new();
        [ObservableProperty]
        private int cellSize = 50;

        [ObservableProperty]
        private int animationSpeed;
        [ObservableProperty]
        private bool animationEnabled;
        [ObservableProperty]
        private bool adaptiveSearchEnabled;

        [ObservableProperty]
        private string? status;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStarted))]
        [NotifyCanExecuteChangedFor(nameof(ClearPathCommand), nameof(ClearWallsCommand), nameof(StopAnimationCommand))]
        private CancellationTokenSource? cancelAnimationTokenSource;

        public bool IsStarted
        {
            get => CancelAnimationTokenSource is not null;
        }

        private Heuristic selectedHeuristic;
        private PathFinder selectedFinder;

        public VisualViewModel()
        {
            const int width = 10;
            const int height = 10;
            AnimationSpeed = 20;
            AnimationEnabled = true;
            CancelAnimationTokenSource = null;

            CellGrid = new CellGridViewModel(new Location(0, 0), new Location(9, 9), width, height);
            cellGrid.PropertyChanged += this.CellGrid_PropertyChanged;
            cellGrid.WalkableChangedEvent += this.CellGrid_WalkableChangedEvent;
        }

        private void CellGrid_WalkableChangedEvent(object? sender, WalkableChangedEventArgs e)
        {
            if (e.IsWalkable)
                Status = $"В клетке [{e.Location.X}, {e.Location.Y}] удалена стена";
            else
                Status = $"В клетке [{e.Location.X}, {e.Location.Y}] установлена стена";

            if (AdaptiveSearchEnabled) RunSearch(); 
        }

        private void CellGrid_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Status = $"{e.PropertyName} изменился";
            if (AdaptiveSearchEnabled) RunSearch();
        }

        [RelayCommand]
        private void HeuristicSelected(string heuristic)
        {
            _ = Enum.TryParse(heuristic, out selectedHeuristic);
            Status = $"Выбранная эвристика: {selectedHeuristic}";
        }

        [RelayCommand]
        private void PathFinderSelected(string finder)
        {
            _ = Enum.TryParse(finder, out selectedFinder);
            Status = $"Выбранный алгоритм: {selectedFinder}";
        }

        [RelayCommand]
        private async void RunSearch()
        {
            StopAnimation();
            ClearPath();

            var walls = CellGrid.Walls;

            // todo: импорт карты из файла/строки?
            var grid = new SquareGrid(CellGrid.Width, CellGrid.Height);
            foreach (var wall in walls)
            {
                grid.SetWalkable(wall, false);
                CellGrid.ChangeCellState(wall, CellState.Wall);
            }

            var goal = CellGrid.Goal;
            var start = CellGrid.Start;
            var result = FindPath(grid, start, goal);

            await StartAnimation(result, goal);
        }

        [RelayCommand(CanExecute = nameof(CanExecuteStopAnimation))]
        private void StopAnimation()
        {
            CancelAnimationTokenSource?.Cancel();
            CancelAnimationTokenSource = null;
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanExecuteStopAnimation()
        {
            return IsStarted;
        }

        private async Task StartAnimation(Dictionary<Location, VisitedLocation> result, Location goal)
        {
            this.CancelAnimationTokenSource = new CancellationTokenSource();
            var cancellationToken = CancelAnimationTokenSource.Token;
            CommandManager.InvalidateRequerySuggested();

            if (AnimationSpeed == 0 || !AnimationEnabled)
                ShowResult(result);
            else
                await AnimateSearchResult(result, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                CreatePath(result, goal);

            StopAnimation();
        }

        private Dictionary<Location, VisitedLocation> FindPath(SquareGrid grid, Location start, Location goal)
        {
            PathFinding.Searchers.PathFinder finder = new AdaptivePathFinder();

            HeuristicFunction? heuristic = null;
            bool calculateDistance = false;

            if (selectedFinder == PathFinder.AStar || selectedFinder == PathFinder.Greedy)
                heuristic = GetHeuristic();

            if (selectedFinder == PathFinder.AStar || selectedFinder == PathFinder.Dijkstra)
                calculateDistance = true;

            var result = finder.FindPath(grid, start, goal, heuristic, calculateDistance);

            return result;
        }

        private HeuristicFunction GetHeuristic()
        {
            return selectedHeuristic switch
            {
                Heuristic.Manhattan => Heuristics.Manhattan,
                Heuristic.Chebyshev => Heuristics.Chebyshev,
                Heuristic.Octile => Heuristics.Octile,
                Heuristic.Euclidean => Heuristics.Euclidean,
                _ => Heuristics.Manhattan,
            };
        }

        private async Task AnimateSearchResult(Dictionary<Location, VisitedLocation> result, CancellationToken cancellationToken)
        {
            foreach ((var key, var value) in result)
            {
                if (cancellationToken.IsCancellationRequested) return;
                var delay = AnimationSpeed == 0 ? 1 : 1000 / AnimationSpeed;
                ChangeCell(key, value);
                await Task.Delay(delay, CancellationToken.None);
            }
        }

        // todo: направления не для диагоналей только, нужно сделать с диагоналями потом
        private readonly int[][] directions = new[]
        {
            new[] {0, 1},
            new[] {0, -1},
            new[] {-1, 0},
            new[] {1, 0},
        };

        private void ChangeCell(Location key, VisitedLocation value)
        {
            if (value.VisitedIndex is null) return;

            CellGrid.ChangeCellState(key, CellState.Visited, false);

            foreach (var direction in directions)
            {
                var location = new Location(key.X + direction[0], key.Y + direction[1]);
                if (CellGrid.GetCell(location)?.State != CellState.Empty) continue;

                this.CellGrid.ChangeCellState(location, CellState.Opened, false);
            }
        }

        private void ShowResult(Dictionary<Location, VisitedLocation> result)
        {
            foreach ((var key, var value) in result)
            {
                var state = value?.VisitedIndex is null ? CellState.Opened : CellState.Visited;

                CellGrid.ChangeCellState(key, state, false);
            }
        }

        private void CreatePath(Dictionary<Location, VisitedLocation> result, Location goal)
        {
            var path = PathUtils.Backtrace(result, goal);
            // путь в качестве линии, а не закрашенных клеток. не знаю, может быть, когда-нибудь.
            // (оно работает, но выглядит плохо)
            // var cellSize = 64;
            // Path = new(path.Select(l => new Point((l.X + 0.5) * cellSize, (l.Y + 0.5) * cellSize)));
            foreach (var location in path)
            {
                CellGrid.ChangeCellState(location, CellState.Path, false);
            }
        }

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecuteClear))]
        private void ClearPath()
        {
            Path = new();
            CellGrid.ClearPath();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteClear))]
        private void ClearWalls()
        {
            ClearPath();
            CellGrid.ClearWalls();
        }
        private bool CanExecuteClear()
        {
            return !IsStarted;
        }
        #endregion
    }
}
