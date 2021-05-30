using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext
    {
        internal event Action<Point, float> onAddToOpenSet;
        internal event Action<Point> onAddToCloseSet;
        internal event Action<Point> onRemoveFromOpenSet;
        internal event Action<Point> onRemoveFromCloseSet;

        internal readonly PointNode.PointNodePool pool = PointNode.Pool();
        internal readonly PriorityQueue<PointNode> openList;
        internal readonly HashSet<Point> closedSet;
        internal readonly HashSet<Point> openSet;
        internal PointNode endingNode;
        internal Point endingPoint;

        internal PathfindingContext()
        {
            this.openList = new PriorityQueue<PointNode>();
            this.closedSet = new HashSet<Point>();
            this.openSet = new HashSet<Point>();
        }

        internal void Reset()
        {
            foreach (var p in this.openList)
            {
                pool.Recycle(p);
            }
            this.endingNode = null;
            this.openSet.Clear();
            this.openList.Clear();
            this.closedSet.Clear();
        }
    }
}
