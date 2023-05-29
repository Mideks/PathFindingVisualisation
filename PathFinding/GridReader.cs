using PathFinding.Graphs;

namespace PathFinding
{
    public static class GridReader
    {

        public static string ReadGridFile(string filePath)
        {
            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Combine all lines into a single string
                string fileData = string.Join("\n", lines);

                // Return the combined file data string
                return fileData;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Console.WriteLine($"Error reading file: {ex.Message}");
                return string.Empty;
            }
        }

        public static SquareGrid StringToGrid(string input)
        {
            // Split the input into lines and remove any leading/trailing whitespace
            string[] lines = input.Trim().Replace(" ", "").Split('\n');

            // Determine the dimensions of the grid
            int width = lines[0].Length;
            int height = lines.Length;

            // Create a new SquareGrid with the specified dimensions
            var grid = new SquareGrid(width, height);

            // Fill the grid with walls and empty spaces based on the input lines
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = lines[y][x];
                    if (c == '#') grid.Walls.Add(new Location(x, y));
                    else if (c == '.') continue; // Do nothing, since the grid is already empty by default
                    else throw new ArgumentException($"Invalid character in input string: ({c})");
                }
            }

            return grid;
        }
    }
}