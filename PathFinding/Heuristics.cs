//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):



namespace PathFinding
{
    public delegate double HeuristicFunction(int dx, int dy);
    /// <summary>
    /// A collection of heuristic functions.
    /// </summary>
    public static class Heuristics
    {
        /// <summary>Manhattan distance.</summary>
        /// <param name="dx">Difference in x.</param>
        /// <param name="dy">Difference in y.</param>
        /// <returns>dx + dy</returns>
        public static double Manhattan(int dx, int dy)
        {
            return dx + dy;
        }

        /// <summary>Euclidean distance.</summary>
        /// <param name="dx">Difference in x.</param>
        /// <param name="dy">Difference in y.</param>
        /// <returns>sqrt(dx * dx + dy * dy)</returns>
        public static double Euclidean(int dx, int dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>Octile distance.</summary>
        /// <param name="dx">Difference in x.</param>
        /// <param name="dy">Difference in y.</param>
        /// <returns>sqrt(dx * dx + dy * dy) for grids</returns>
        public static double Octile(int dx, int dy)
        {
            var F = Math.Sqrt(2) - 1;
            return (dx < dy) ? F * dx + dy : F * dy + dx;
        }

        /// <summary>Chebyshev distance.</summary>
        /// <param name="dx">Difference in x.</param>
        /// <param name="dy">Difference in y.</param>
        /// <returns>max(dx, dy)</returns>
        public static double Chebyshev(int dx, int dy)
        {
            return Math.Max(dx, dy);
        }
    }
}
