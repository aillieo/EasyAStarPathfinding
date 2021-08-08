using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext<T> where T : IGraphNode
    {
        internal readonly NodePointer<T>.NodePointerPool pool = NodePointer<T>.Pool();
        internal readonly UniquePriorityQueue<NodePointer<T>> openList;
        internal readonly Dictionary<T, NodePointer<T>> closedSet;
        internal readonly HashSet<T> openSet;
        internal NodePointer<T> endingPointer;
        internal T startingNode;
        internal T endingNode;
        internal readonly IGraphData<T> graphData;
        internal readonly Algorithms algorithm;

        internal PathfindingContext(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new UniquePriorityQueue<NodePointer<T>>();
            this.closedSet = new Dictionary<T, NodePointer<T>>();
            this.openSet = new HashSet<T>();
        }

        internal NodePointer<T> GetPointNode(T gird = default, NodePointer<T> parent = default)
        {
            return this.pool.GetPointNode(gird, parent);
        }

        internal void Reset()
        {
            foreach (var p in this.openList)
            {
                pool.Recycle(p);
            }
            this.endingPointer = null;
            this.openSet.Clear();
            this.openList.Clear();
            this.closedSet.Clear();
        }

        internal bool IsEndingNode(T node)
        {
            return object.ReferenceEquals(node, endingNode);
        }
    }
}
