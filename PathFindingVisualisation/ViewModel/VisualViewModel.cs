using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PathFinding;
using PathFindingVisualisation.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PathFindingVisualisation.ViewModel
{
    public partial class VisualViewModel : ObservableObject
    {
        [ObservableProperty]
        private CellGridViewModel cellGrid = new();
        [ObservableProperty]
        public PointCollection path = new();

        public VisualViewModel()
        {
            RunSearch();
            Clear();
        }

        [RelayCommand]
        private void Clear()
        {
            foreach (var cell in CellGrid.Cells)
            {
                if (cell.State == CellState.Visited ||
                    cell.State == CellState.Opened ||
                    cell.State == CellState.Path)
                    CellGrid.ChangeCellState(new Location(cell.X, cell.Y), CellState.Empty);
            }
            Path = new();
            CellGrid.Clear();
        }

        [RelayCommand]
        private void RunSearch()
        {
            Clear();

            // хз может подругому сделать
            var walls = CellGrid.GetWalls();

            // todo: убрать константы
            var width = 25;
            var height = 25;

            var grid = new SquareGrid(width, height);
            foreach (var wall in walls)
            {
                grid.SetWalkable(wall, false);
                CellGrid.ChangeCellState(wall, CellState.Wall);
            }

            // todo: добавление выбора стартовой и конечной точки
            var start = new Location(0, 0);
            var goal = new Location(9, 9);

            //grid = GridFromString();
            PathFinder searcher = GetFinder();

            // todo: показывать путь
            // todo: отображение результатов в виде анимации
            var result = searcher.FindPath(grid, start, goal, Heuristics.Manhattan);
            foreach ((var key, var value) in result)
            {
                var state = value.VisitedIndex is null ? CellState.Opened : CellState.Visited;
                CellGrid.ChangeCellState(key, state);
            }

            CreatePath(result, goal);

            CellGrid.ChangeCellState(start, CellState.Start);
            CellGrid.ChangeCellState(goal, CellState.End);
        }

        private void CreatePath(Dictionary<Location, VisitedLocation> result, Location goal)
        {
            var path = Utils.Backtrace(result, goal);
            // путь в качестве линии, а не закрашеных клеток. не знаю, может быть, когда-нибудь. (оно работает)
            /*var cellSize = 64;
            Path = new(Utils.Backtrace(result, goal).Select(l => new Point((l.X + 0.5) * cellSize, (l.Y + 0.5) * cellSize)));
*/
            foreach (var location in path)
            {
                CellGrid.ChangeCellState(location, CellState.Path);
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
    }
}
