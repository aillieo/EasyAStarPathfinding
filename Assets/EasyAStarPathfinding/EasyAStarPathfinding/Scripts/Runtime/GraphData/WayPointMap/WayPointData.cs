using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class WayPointData : IWayPointData
    {
        public IEnumerable<Point> CollectNeighbor(Point current)
        {
            throw new System.NotImplementedException();
        }

        public float DefaultHeuristicFunc(Point node1, Point node2)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Point> GetAllNodes()
        {
            throw new System.NotImplementedException();
        }
    }
}
