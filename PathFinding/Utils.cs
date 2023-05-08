using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class Utils
    {
        public static List<Location> Backtrace(Dictionary<Location, VisitedLocation> searchResult, Location goal)
        {
            if (!searchResult.ContainsKey(goal)) return new List<Location>(); 

            var trace = new List<Location> { goal };
            while (!goal.Equals(searchResult[goal].CameFrom))
            {
                goal = searchResult[goal].CameFrom;
                trace.Add(goal);
            }
            trace.Reverse();
            return trace;
        }
    }
}
