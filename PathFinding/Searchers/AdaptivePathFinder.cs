//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

using PathFinding.Graphs;

namespace PathFinding.Searchers
{
    public class AdaptivePathFinder : PathFinder
    {
        public override Dictionary<Location, VisitedLocation> FindPath(
            IWeightedGraph<Location> graph, Location start, Location goal, HeuristicFunction? heuristic, bool calculateDistance)
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
                    double newCost = 
                        calculateDistance ?
                        visited[current].CostSoFar + graph.Cost(current, next) : 0;

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
                        var h = heuristic?.Invoke(dx, dy) ?? 0;

                        location.CalculatedHeuristic = h;
                        double priority = newCost + h;

                        frontier.Enqueue(next, priority);
                    }
                }
            }

            return visited;
        }
    }
}


