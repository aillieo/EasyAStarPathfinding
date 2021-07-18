using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext
    {
        internal readonly GridNode.PointNodePool pool = GridNode.Pool();
        internal readonly PriorityQueue<GridNode> openList;
        internal readonly HashSet<Grid> closedSet;
        internal readonly HashSet<Grid> openSet;
        internal GridNode endingNode;
        internal Grid startPoint;
        internal Grid endingPoint;
        internal readonly IGridMapData graphData;
        internal readonly Algorithms algorithm;

        internal PathfindingContext(IGridMapData graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new PriorityQueue<GridNode>();
            this.closedSet = new HashSet<Grid>();
            this.openSet = new HashSet<Grid>();
        }

        internal GridNode GetPointNode(Grid gird = default, GridNode parent = default)
        {
            return this.pool.GetPointNode(gird, parent);
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
