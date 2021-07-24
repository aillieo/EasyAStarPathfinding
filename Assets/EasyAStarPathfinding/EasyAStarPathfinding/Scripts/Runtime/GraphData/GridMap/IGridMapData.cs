using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public interface IGridMapData : IGraphData<Grid>
    {
        float GetCost(int x, int y);
    }
}
