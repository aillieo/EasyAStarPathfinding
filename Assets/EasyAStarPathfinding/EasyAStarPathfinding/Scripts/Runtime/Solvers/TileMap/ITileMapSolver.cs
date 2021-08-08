using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface ITileMapSolver : ISolver
    {
        void Init(Tile startPoint, Tile endPoint);

        void GetResult(List<Tile> toFill);
    }
}
