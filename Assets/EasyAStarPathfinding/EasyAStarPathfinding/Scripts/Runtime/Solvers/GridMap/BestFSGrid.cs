using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class BestFSGrid : BestFS<Grid>, IGridMapSolver
    {
        public BestFSGrid(IGraphData<Grid> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float HeuristicFunc(Grid nodeFrom, Grid nodeTo)
        {
            return HeuristicFuncPreset.ManhattanDist(nodeFrom, nodeTo);
        }
    }
}
