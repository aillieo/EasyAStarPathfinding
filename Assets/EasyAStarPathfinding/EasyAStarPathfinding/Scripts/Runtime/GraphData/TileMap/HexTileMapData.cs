using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class HexTileMapData : ITileMapData
    {
        public IEnumerable<Tile> CollectNeighbor(Tile current)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tile> GetAllNodes()
        {
            throw new NotImplementedException();
        }

        public float GetCost(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
