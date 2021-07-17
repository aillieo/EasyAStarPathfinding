using System;

namespace AillieoUtils.Pathfinding
{
    public interface ISolver
    {
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
                case IGridData gridData:
                    return CreateForGridData(gridData, algorithm);
            }

            throw new NotImplementedException();
        }

        private static ISolver CreateForGridData(IGridData gridData, Algorithms algorithm)
        {
            return new AStar();
        }
    }
}
