using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public interface IGridData : IGraphData<Point>
    {
        bool Passable(int x, int y);
    }
}
