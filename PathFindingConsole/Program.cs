using PathFinding;
using PathFinding.Graphs;
using PathFinding.Searchers;
using System.Diagnostics;
using System.Text;

namespace PathFindingConsole
{
    enum RenderType
    {
        Arrows,
        VisitedIndex,
        CostSoFar,
        Heuristic,
        TotalCost,
        Explored,
        PathOnly
    }
    public class RunAStar
    {
        static void Main()
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

            var gridTxt = GridReader.ReadGridFile(@"C:\Users\Lenovo\Desktop\mom\output.txt");

            SquareGrid grid = GridReader.StringToGrid(gridTxt);

            // Run A*
            var start = new Location(0, 0);
            var goal = new Location(35, 30);

            var astar = new AdaptivePathFinder();

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            var searchResult = astar.FindPath(grid, start, goal, Heuristics.Manhattan, true);
            //stopwatch.Stop();

            for (var limit = 0; limit < searchResult.Max(v => v.Value.VisitedIndex) + 1; limit++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(DrawGrid(grid, searchResult, start, goal, limit, RenderType.Arrows));
                Console.WriteLine();
                //Console.WriteLine(CreateMap(grid, astar, limit));
                Thread.Sleep(100);
            }
            //Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        }

        static string DrawGrid(
            SquareGrid grid, Dictionary<Location, VisitedLocation> searchResult,
            Location start, Location goal, int limit = int.MaxValue,
            RenderType renderType = RenderType.Arrows)
        {
            var defaultSymbol = "░░";
            var wallSymbol = "██";
            var visitedSymbol = "▓▓";
            var neighborSymbol = "▒▒";

            // создаем двумерный массив и устанавливаем символ по умолчанию для всех ячеек
            var matrix = Enumerable.Range(0, grid.Height)
                .Select(x => Enumerable.Repeat(defaultSymbol, grid.Width).ToArray())
                .ToArray();


            foreach (var id in grid.Walls)
                matrix[id.Y][id.X] = wallSymbol;

            if (renderType == RenderType.Explored)
            {
                foreach (var loc in searchResult
                    .Where(x => x.Value.VisitedIndex != null)
                    .OrderBy(x => x.Value.VisitedIndex)
                    .Take(limit))
                {
                    matrix[loc.Key.Y][loc.Key.X] = visitedSymbol;
                    foreach (var neighbor in grid.Neighbors(loc.Key))
                        if (!grid.Walls.Contains(neighbor) && matrix[neighbor.Y][neighbor.X] != visitedSymbol)
                            matrix[neighbor.Y][neighbor.X] = neighborSymbol;
                }
            }
            else if (renderType == RenderType.PathOnly)
            {
                var trace = PathUtils.Backtrace(searchResult, goal);
                for (int i = 0; i < trace.Count - 1; i++)
                    matrix[trace[i].Y][trace[i].X] = GetDirectionSymbol(trace[i], trace[i + 1]);
            }
            else
            {
                foreach ((var key, var value) in searchResult)
                {
                    var cellSymbol = renderType switch
                    {
                        RenderType.VisitedIndex => $"{value.VisitedIndex:D2}",
                        RenderType.CostSoFar => $"{value.CostSoFar:F0}",
                        RenderType.Heuristic => $"{value.CalculatedHeuristic:F0}",
                        RenderType.TotalCost => $"{value.CalculatedHeuristic + value.CostSoFar:F0}",
                        RenderType.Arrows => GetDirectionSymbol(value.CameFrom, key),
                        _ => throw new ArgumentOutOfRangeException(nameof(renderType), renderType, null)
                    };

                    if (value.VisitedIndex is null) cellSymbol = "*";
                    else if (value.VisitedIndex > limit) cellSymbol = "·";

                    matrix[key.Y][key.X] = cellSymbol;
                }
            }

            static string GetDirectionSymbol(Location from, Location to)
            {
                if (to.X == from.X + 1) return ">";
                else if (to.X == from.X - 1) return "<";
                else if (to.Y == from.Y + 1) return "↓";
                else if (to.Y == from.Y - 1) return "↑";
                else if (to.Y == from.Y && to.X == from.X) return ".";
                else throw new ArgumentException($"Incorrect combination of points: ({from.X}, {from.Y}) and ({to.X}, {to.Y})");
            }

            matrix[start.Y][start.X] = "A";
            matrix[goal.Y][goal.X] = "Я";

            return string.Join("\n", matrix.Select(x => string.Join("", x.Select(y => y.PadRight(3, ' ')))));
        }
    }
}