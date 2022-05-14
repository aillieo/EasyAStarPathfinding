using System;

namespace AillieoUtils.Pathfinding
{
    internal static class SolverCreator
    {
        internal static ISolver<T> Create<T>(IGraphData<T> graphData, Algorithms algorithm)
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
                    return new AStar<Tile>(tileData, algorithm);
                case Algorithms.DijkstraAlgorithm:
                    return new Dijkstra<Tile>(tileData, algorithm);
                case Algorithms.ThetaStar:
                    return new ThetaStar<Tile>(tileData, algorithm);
                case Algorithms.HPA:
                    return new HPA<Tile>(tileData, algorithm);

                case Algorithms.LPAStar:
                    return new LPAStar<Tile>(tileData, algorithm);
                case Algorithms.DStarLite:
                    return new DStarLite<Tile>(tileData, algorithm);
            }

            throw new NotImplementedException();
        }
    }
}
