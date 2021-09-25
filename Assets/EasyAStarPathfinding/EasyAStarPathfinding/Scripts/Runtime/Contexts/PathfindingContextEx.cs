using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContextEx<T> : IPathfindingContext<T, NodeWrapperEx<T>> where T : IGraphNode
    {
        internal readonly UniquePriorityQueue<NodeWrapperEx<T>> openList;
        private readonly Dictionary<T, NodeWrapperEx<T>> nodeMapping;
        //internal T startingNode;
        //internal T endingNode;
        private readonly IGraphData<T> graphData;
        private readonly Algorithms algorithm;

        internal PathfindingContextEx(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new UniquePriorityQueue<NodeWrapperEx<T>>(Comparer<NodeWrapperEx<T>>.Create((o1, o2) => o2.CompareTo(o1)));
            this.nodeMapping = new Dictionary<T, NodeWrapperEx<T>>();
        }

        public NodeWrapperEx<T> GetOrCreateNode(T nodeData)
        {
            if (!nodeMapping.TryGetValue(nodeData, out NodeWrapperEx<T> nodeWrapperEx))
            {
                nodeWrapperEx = new NodeWrapperEx<T>(nodeData);
                nodeMapping.Add(nodeData, nodeWrapperEx);
            }

            return nodeWrapperEx;
        }

        public void Reset()
        {
            this.nodeMapping.Clear();
            this.openList.Clear();
        }

        public NodeWrapperEx<T> TryGetNode(T nodeData)
        {
            NodeWrapperEx<T> node = null;
            if (this.nodeMapping.TryGetValue(nodeData, out node))
            {
                return node;
            }

            return null;
        }

        public IGraphData<T> GetGraphData()
        {
            return this.graphData;
        }

        public IEnumerable<NodeWrapperEx<T>> GetAllNodes()
        {
            foreach (var pair in nodeMapping)
            {
                yield return pair.Value;
            }
        }
    }
}
