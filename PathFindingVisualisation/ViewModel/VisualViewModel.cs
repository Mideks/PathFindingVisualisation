using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PathFinding;
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
        private string? status;

        private CancellationTokenSource? cancelAnimationTokenSource;

        private Heuristic selectedHeuristic;

        public VisualViewModel()
        {
            const int width = 10;
            const int height = 10;
            AnimationSpeed = 20;

            CellGrid = new CellGridViewModel(new Location(0, 0), new Location(9, 9), width, height);
            //RunSearch();
            //ClearPath();

            cellGrid.PropertyChanged += this.CellGrid_PropertyChanged;
        }

        private void CellGrid_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Status = $"{e.PropertyName} изменился";
        }

        [RelayCommand]
        private void HeuristicSelected(string heuristic)
        {
            _ = Enum.TryParse(heuristic, out selectedHeuristic);
            Status = $"Выбранная эвристика: {selectedHeuristic}";
        }

        [RelayCommand]
        private async void RunSearch()
        {
            StopAnimation();
            ClearPath();

            var walls = CellGrid.Walls;

            // todo: импорт карты из файла/строки?
            //var grid = GridFromString();
            var grid = new SquareGrid(CellGrid.Width, CellGrid.Height);
            foreach (var wall in walls)
            {
                grid.SetWalkable(wall, false);
                CellGrid.ChangeCellState(wall, CellState.Wall);
            }

            var goal = CellGrid.Goal;
            var searcher = GetFinder();
            var start = CellGrid.Start;
            var heuristic = GetHeuristic();
            var result = searcher.FindPath(grid, start, goal, heuristic);

            await StartAnimation(result, goal);

            //CellGrid.ChangeCellState(start, CellState.Start);
            //CellGrid.ChangeCellState(goal, CellState.Goal);
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

        private async Task StartAnimation(Dictionary<Location, VisitedLocation> result, Location goal)
        {
            this.cancelAnimationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancelAnimationTokenSource.Token;

            if (AnimationSpeed == 0)
                ShowResult(result);
            else
                await AnimateSearchResult(result, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                CreatePath(result, goal);
        }

        private void StopAnimation()
        {
            cancelAnimationTokenSource?.Cancel();
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

        // todo: направления не для диаганалей только, нужно сделать с диаганалями потом
        private readonly int[][] directions = new[]
        {
            new[] {0, 1},
            new[] {0, -1},
            new[] {-1, 0},
            new[] {1, 0},
        };

        private void ChangeCell(Location key, VisitedLocation value)
        {

            if (value.VisitedIndex is not null)
            {
                CellGrid.ChangeCellState(key, CellState.Visited, false);

                foreach (var direction in directions)
                {
                    var location = new Location(key.X + direction[0], key.Y + direction[1]);
                    if (CellGrid.GetCell(location)?.State != CellState.Empty) continue;

                    this.CellGrid.ChangeCellState(location, CellState.Opened, false);
                }
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
            var path = Utils.Backtrace(result, goal);
            // путь в качестве линии, а не закрашеных клеток. не знаю, может быть, когда-нибудь. (оно работает, но выглядит плохо)
            /*var cellSize = 64;
            Path = new(Utils.Backtrace(result, goal).Select(l => new Point((l.X + 0.5) * cellSize, (l.Y + 0.5) * cellSize)));
*/
            foreach (var location in path)
            {
                CellGrid.ChangeCellState(location, CellState.Path, false);
            }
        }

        private static PathFinder GetFinder()
        {
            // todo: добавление выбора алгоритма
            return new AStarSearch();
        }

        private SquareGrid GridFromString()
        {
            string input =
                ". . . . . . . . . .\n" +
                ". . . . . . . . . .\n" +
                ". . . . . . . . . .\n" +
                ". . . . . # # . . .\n" +
                ". . # . . . # . . .\n" +
                ". . # . . . # . . .\n" +
                ". . . . . . # . . .\n" +
                ". . # # # . # . . .\n" +
                ". . . . # . . . . .\n" +
                ". . . . . . . . . .\n";
            var grid = GridReader.StringToGrid(input);
            foreach (var wall in grid.Walls)
            {
                CellGrid.ChangeCellState(wall, CellState.Wall);
            }
            return grid;
        }

        #region Commands
        [RelayCommand]
        private void ClearPath()
        {
            Path = new();
            CellGrid.ClearPath();
        }

        [RelayCommand]
        private void ClearWalls()
        {
            ClearPath();
            CellGrid.ClearWalls();
        }
        #endregion
    }
}
