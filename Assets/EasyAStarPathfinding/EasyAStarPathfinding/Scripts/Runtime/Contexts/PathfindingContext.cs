using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext<T> : IPathfindingContext<T> where T : IGraphNode
    {
        internal readonly UniquePriorityQueue<NodePointer<T>> openList;
        internal readonly Dictionary<T, NodePointer<T>> closedSet;
        internal readonly Dictionary<T, NodePointer<T>> openSet;
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
            this.openSet = new Dictionary<T, NodePointer<T>>();
        }

        internal NodePointer<T> GetNode(T node, NodePointer<T> previous = default)
        {
            return new NodePointer<T>(node, previous);
        }

        internal void Reset()
        {
            this.endingPointer = null;
            this.openSet.Clear();
            this.openList.Clear();
            this.closedSet.Clear();
        }

        public bool IsEndingNode(T node)
        {
            return object.ReferenceEquals(node, endingNode);
        }

        public NodePointer<T> TryGetOpenNode(T nodeData)
        {
            NodePointer<T> node = null;
            this.openSet.TryGetValue(nodeData, out node);
            return node;
        }

        public NodePointer<T> TryGetClosedNode(T nodeData)
        {
            NodePointer<T> node = null;
            this.closedSet.TryGetValue(nodeData, out node);
            return node;
        }

        public IGraphData<T> GetGraphData()
        {
            return this.graphData;
        }
    }
}
