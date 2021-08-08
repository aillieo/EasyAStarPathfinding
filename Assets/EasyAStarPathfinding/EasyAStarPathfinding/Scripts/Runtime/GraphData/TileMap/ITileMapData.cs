using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public interface ITileMapData : IGraphData<Tile>
    {
        float GetCost(int x, int y);
    }
}
