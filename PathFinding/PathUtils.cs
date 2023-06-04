using PathFinding;
using PathFinding.Searchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class PathUtils
    {
        public static List<Location> Backtrace(Dictionary<Location, VisitedLocation> searchResult, Location goal)
        {
            if (!searchResult.ContainsKey(goal)) return new List<Location>();

            var trace = new List<Location> { goal };
            var current = goal;
            while (!current.Equals(searchResult[current].CameFrom))
            {
                current = searchResult[current].CameFrom;
                trace.Add(current);
            }
            trace.Reverse();
            return trace;
        }

    }
}


