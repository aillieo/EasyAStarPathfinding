using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class HexGridMapData : IGridMapData
    {
        public IEnumerable<Grid> CollectNeighbor(Grid current)
        {
            throw new NotImplementedException();
        }

        public float GetCost(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
