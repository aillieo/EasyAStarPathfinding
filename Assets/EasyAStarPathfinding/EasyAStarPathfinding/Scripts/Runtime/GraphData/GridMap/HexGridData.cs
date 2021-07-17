using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class HexGridData : IGridData
    {
        public IEnumerable<Point> CollectNeighbor(Point current)
        {
            throw new NotImplementedException();
        }

        public bool Passable(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
