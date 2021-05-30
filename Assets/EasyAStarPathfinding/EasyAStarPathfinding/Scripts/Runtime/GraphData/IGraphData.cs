using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IGraphData
    {
        IEnumerable<Point> CollectNeighbor(Point current);
    }
}
