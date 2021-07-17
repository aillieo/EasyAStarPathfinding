using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext
    {
        internal readonly PointNode.PointNodePool pool = PointNode.Pool();
        internal readonly PriorityQueue<PointNode> openList;
        internal readonly HashSet<Point> closedSet;
        internal readonly HashSet<Point> openSet;
        internal PointNode endingNode;
        internal Point startPoint;
        internal Point endingPoint;
        internal readonly IGridData graphData;
        internal readonly Algorithms algorithm;

        internal PathfindingContext(IGridData graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new PriorityQueue<PointNode>();
            this.closedSet = new HashSet<Point>();
            this.openSet = new HashSet<Point>();
        }

        internal PointNode GetPointNode(Point point = default, PointNode parent = default)
        {
            return this.pool.GetPointNode(point, parent);
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
