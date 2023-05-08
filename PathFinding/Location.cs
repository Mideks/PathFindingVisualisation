//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

namespace PathFinding
{
    public struct Location
    {
        // Implementation notes: I am using the default Equals but it can
        // be slow. You'll probably want to override both Equals and
        // GetHashCode in a real project.

        public int X { get; init; }
        public int Y { get; init; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}