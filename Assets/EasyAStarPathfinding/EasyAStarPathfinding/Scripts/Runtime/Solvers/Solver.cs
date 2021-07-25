using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface ISolver
    {
        PathfindingState state { get; }

        void Init();

        PathfindingState Step();

        void CleanUp();
    }

    public static class Solvers
    {
        public static ISolver Create(IGraphData graphData, Algorithms algorithm)
        {
            switch (graphData)
            {
                case IGridMapData gridData:
                    return CreateForGridData(gridData, algorithm);
            }

            throw new NotImplementedException();
        }

        private static IGridMapSolver CreateForGridData(IGridMapData gridData, Algorithms algorithm)
        {
            switch (algorithm)
            {
                case Algorithms.AStar:
                    return new AStarGrid(gridData, algorithm);
                case Algorithms.DepthFirstSearch:
                    return new DepthFSGrid(gridData, algorithm);
                case Algorithms.BreadthFirstSearch:
                    return new BreadthFSGrid(gridData, algorithm);
                case Algorithms.BestFirstSearch:
                    return new BestFSGrid(gridData, algorithm);
                case Algorithms.DijkstraAlgorithm:
                    return new DijkstraGrid(gridData, algorithm);
            }

            throw new NotImplementedException();
        }
    }
}
