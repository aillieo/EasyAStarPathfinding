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

        public float DefaultHeuristicFunc(Tile node1, Tile node2)
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

        public bool LineOfSight(Tile node1, Tile node2)
        {
            throw new NotImplementedException();
        }
    }
}
