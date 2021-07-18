using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class DepthFSGrid : DepthFS, IGridMapSolver
    {
        public DepthFSGrid(IGridMapData gridData, Algorithms algorithm)
            : base(gridData, algorithm)
        {
        }

        public void GetResult(List<Grid> toFill)
        {
            throw new NotImplementedException();
        }

        public void Init(Grid startPoint, Grid endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
