using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class AStarGrid : AStar<Grid>, IGridMapSolver
    {
        public AStarGrid(IGridMapData gridData, Algorithms algorithm)
            : base(gridData, algorithm)
        {
        }

        protected override float HeuristicFunc(Grid nodeFrom, Grid nodeTo)
        {
            return HeuristicFuncPreset.ManhattanDist(nodeFrom, nodeTo);
        }
    }
}
