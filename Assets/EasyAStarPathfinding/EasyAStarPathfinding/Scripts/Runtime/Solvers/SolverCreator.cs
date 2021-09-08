using System;

namespace AillieoUtils.Pathfinding
{
    internal static class SolverCreator
    {
        internal static ISolver<IGraphNode> Create(IGraphData graphData, Algorithms algorithm)
        {
            switch (graphData)
            {
                case ITileMapData tileMapData:
                    return CreateForTileData(tileMapData, algorithm);
            }

            throw new NotImplementedException();
        }

        private static ISolver<Tile> CreateForTileData(ITileMapData tileData, Algorithms algorithm)
        {
            switch (algorithm)
            {
                case Algorithms.AStar:
                    return new AStarTile(tileData, algorithm);
                case Algorithms.DepthFirstSearch:
                    return new DepthFSTile(tileData, algorithm);
                case Algorithms.BreadthFirstSearch:
                    return new BreadthFSTile(tileData, algorithm);
                case Algorithms.BestFirstSearch:
                    return new BestFSTile(tileData, algorithm);
                case Algorithms.DijkstraAlgorithm:
                    return new DijkstraTile(tileData, algorithm);

                case Algorithms.LPAStar:
                    return new LPAStarTile(tileData, algorithm);
            }

            throw new NotImplementedException();
        }
    }
}
