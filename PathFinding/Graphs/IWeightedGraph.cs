//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

// A* needs only a WeightedGraph and a location type L, and does *not*
// have to be a grid. However, in the example code I am using a grid.

namespace PathFinding.Graphs
{
    public interface IWeightedGraph<L>
    {
        double Cost(Location a, Location b);
        IEnumerable<Location> Neighbors(Location id);
    }
}