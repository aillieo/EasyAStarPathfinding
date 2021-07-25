using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class DijkstraGrid : Dijkstra<Grid>, IGridMapSolver
    {
        public DijkstraGrid(IGraphData<Grid> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float HeuristicFunc(Grid nodeFrom, Grid nodeTo)
        {
            return HeuristicFuncPreset.ManhattanDist(nodeFrom, nodeTo);
        }
    }
}
