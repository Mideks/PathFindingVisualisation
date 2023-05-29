//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

using PathFinding.Graphs;

namespace PathFinding.Searchers
{
    public class VisitedLocation
    {
        public Location CameFrom { get; set; }
        public int? VisitedIndex { get; set; }
        public double CostSoFar { get; set; } = double.MaxValue;
        public double Heuristic { get; set; }
    }

    public class AStarSearch : PathFinder
    {
        public override Dictionary<Location, VisitedLocation> FindPath(
            IWeightedGraph<Location> graph, Location start, Location goal, HeuristicFunction heuristic)
        {
            var visited = new Dictionary<Location, VisitedLocation>();
            var frontier = new PriorityQueue<Location, double>();
            var visitedIndex = 0;

            frontier.Enqueue(start, 0);
            visited[start] = new()
            {
                CameFrom = start,
                VisitedIndex = visitedIndex++,
                CostSoFar = 0
            };

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Equals(goal)) break;
                visited[current].VisitedIndex ??= visitedIndex++;

                foreach (var next in graph.Neighbors(current))
                {
                    double newCost = visited[current].CostSoFar + graph.Cost(current, next);

                    if (!visited.TryGetValue(next, out var location))
                    {
                        location = new();
                        visited[next] = location;
                    }

                    if (newCost < location.CostSoFar)
                    {
                        location.CostSoFar = newCost;
                        location.CameFrom = current;

                        var dx = Math.Abs(next.X - goal.X);
                        var dy = Math.Abs(next.Y - goal.Y);

                        var h = heuristic(dx, dy);
                        location.Heuristic = h;
                        double priority = newCost + h;

                        frontier.Enqueue(next, priority);
                    }
                }
            }

            return visited;
        }
    }
}


