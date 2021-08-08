using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class IsometricTileMapData : ITileMapData
    {
        public IEnumerable<Tile> CollectNeighbor(Tile current)
        {
            throw new NotImplementedException();
        }

        public float GetCost(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
