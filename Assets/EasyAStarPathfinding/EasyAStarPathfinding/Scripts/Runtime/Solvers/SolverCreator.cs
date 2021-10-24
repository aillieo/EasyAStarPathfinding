using System;

namespace AillieoUtils.Pathfinding
{
    internal static class SolverCreator
    {
        internal static ISolver<T> Create<T>(IGraphData<T> graphData, Algorithms algorithm) where T : IGraphNode
        {
            switch (graphData)
            {
                case ITileMapData tileMapData:
                    return CreateForTileData(tileMapData, algorithm) as ISolver<T>;
            }

            throw new NotImplementedException();
        }

        private static ISolver<Tile> CreateForTileData(ITileMapData tileData, Algorithms algorithm)
        {
            switch (algorithm)
            {
                case Algorithms.AStar:
                    return new AStarTile(tileData, algorithm);
                case Algorithms.DijkstraAlgorithm:
                    return new DijkstraTile(tileData, algorithm);
                case Algorithms.ThetaStar:
                    return new ThetaStarTile(tileData, algorithm);

                case Algorithms.LPAStar:
                    return new LPAStarTile(tileData, algorithm);
                case Algorithms.DStarLite:
                    return new DStarLiteTile(tileData, algorithm);
            }

            throw new NotImplementedException();
        }
    }
}
