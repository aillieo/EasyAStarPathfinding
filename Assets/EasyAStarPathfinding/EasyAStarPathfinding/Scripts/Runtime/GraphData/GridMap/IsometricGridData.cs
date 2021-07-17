using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class IsometricGridData : IGridData
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
