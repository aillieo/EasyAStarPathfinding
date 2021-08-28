using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface ITileMapSolver : ISolver<Tile>
    {
        void Init(Tile startPoint, Tile endPoint);

        void GetResult(List<Tile> toFill);
    }
}
