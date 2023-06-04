//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

namespace PathFinding.Searchers
{
    public class VisitedLocation
    {
        public Location CameFrom { get; set; }
        public int? VisitedIndex { get; set; }
        public double CostSoFar { get; set; } = double.MaxValue;
        public double CalculatedHeuristic { get; set; }
    }
}


