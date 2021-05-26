using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IGridData : IGraphData
    {
        bool Passable(int x, int y);
        IEnumerable<Point> CollectNeighbor(Point current);
    }
}
