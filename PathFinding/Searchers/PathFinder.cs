﻿//Here’s a graph representing a grid with weighted edges (the forest and walls example from the main page):

using PathFinding.Graphs;

namespace PathFinding.Searchers
{
    public abstract class PathFinder
    {
        public abstract Dictionary<Location, VisitedLocation> FindPath(
            IWeightedGraph<Location> graph, Location start, Location goal, HeuristicFunction? heuristic, bool calculateDistance);
    }
}