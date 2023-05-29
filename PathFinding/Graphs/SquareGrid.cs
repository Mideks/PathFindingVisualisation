//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

namespace PathFinding.Graphs
{
    public class SquareGrid : IWeightedGraph<Location>
    {
        // Implementation notes: I made the fields public for convenience,
        // but in a real project you'll probably want to follow standard
        // style and make them private.

        private static readonly Location[] Directions = new[]
        {
            new Location(1, 0),
            new Location(0, -1),
            new Location(-1, 0),
            new Location(0, 1)
        };


        public HashSet<Location> Walls = new();

        public int Width { get; set; }
        public int Height { get; set; }

        public SquareGrid(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void SetWalkable(Location location, bool walkable)
        {
            if (walkable == false)
            {
                Walls.Add(location);
            }
            else
            {
                Walls.Remove(location);
            }
        }

        public bool InBounds(Location id)
        {
            return 0 <= id.X && id.X < Width
                && 0 <= id.Y && id.Y < Height;
        }

        public bool Passable(Location id)
        {
            return !Walls.Contains(id);
        }

        public double Cost(Location a, Location b)
        {
            return 1;

            // В случае диагоналей ето :
            /*      int dx = Math.Abs(a.X - b.X);
                    int dy = Math.Abs(a.Y - b.Y);

                    return Math.Max(dx,dy) + (Math.Sqrt(2) - 1) * Math.Min(dx, dy);*/
        }


        // Todo: Возвращает только "прямых" соседей. для диагональных не будет
        // можно было бы просто изменить Directions, но это уберёт гибкость.
        // Нужно добавить в качестве параметра, можно ли диагонали,
        // И если можно, то в каком случае. 

        public IEnumerable<Location> Neighbors(Location id)
        {
            foreach (var direction in Directions)
            {
                var next = new Location(id.X + direction.X, id.Y + direction.Y);
                if (InBounds(next) && Passable(next))
                    yield return next;
            }
        }
    }
}