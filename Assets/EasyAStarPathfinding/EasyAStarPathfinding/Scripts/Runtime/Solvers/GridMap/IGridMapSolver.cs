using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IGridMapSolver : ISolver
    {
        void Init(Grid startPoint, Grid endPoint);

        void GetResult(List<Grid> toFill);
    }
}
