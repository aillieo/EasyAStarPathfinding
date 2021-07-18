using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public interface IGridMapData : IGraphData<Grid>
    {
        bool Passable(int x, int y);
    }
}
