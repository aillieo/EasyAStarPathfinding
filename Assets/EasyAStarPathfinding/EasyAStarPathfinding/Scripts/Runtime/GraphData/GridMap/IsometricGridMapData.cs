using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class IsometricGridMapData : IGridMapData
    {
        public IEnumerable<Grid> CollectNeighbor(Grid current)
        {
            throw new NotImplementedException();
        }

        public bool Passable(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
