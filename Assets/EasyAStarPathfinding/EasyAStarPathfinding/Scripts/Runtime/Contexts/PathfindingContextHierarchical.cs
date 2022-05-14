using AillieoUtils.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContextHierarchical<T> : IPathfindingContext<T, NodeWrapper<T>>
    {
        internal readonly UniquePriorityQueue<NodeWrapper<T>> openList;
        private readonly Dictionary<T, NodeWrapper<T>> nodeMapping;
        //internal T startingNode;
        //internal T endingNode;
        private readonly IHierarchicalGraphData<T> graphData;
        private readonly Algorithms algorithm;

        internal PathfindingContextHierarchical(IHierarchicalGraphData<T> graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new UniquePriorityQueue<NodeWrapper<T>>(Comparer<NodeWrapper<T>>.Create((o1, o2) => o2.CompareTo(o1)));
            this.nodeMapping = new Dictionary<T, NodeWrapper<T>>();
        }

        public NodeWrapper<T> GetOrCreateNode(T nodeData)
        {
            if (!nodeMapping.TryGetValue(nodeData, out NodeWrapper<T> nodeWrapper))
            {
                nodeWrapper = new NodeWrapper<T>(nodeData);
                nodeMapping.Add(nodeData, nodeWrapper);
            }

            return nodeWrapper;
        }

        public void Reset()
        {
            this.nodeMapping.Clear();
            this.openList.Clear();
        }

        public NodeWrapper<T> TryGetNode(T nodeData)
        {
            NodeWrapper<T> node = null;
            if (this.nodeMapping.TryGetValue(nodeData, out node))
            {
                return node;
            }

            return null;
        }

        public IGraphData<T> GetGraphData()
        {
            return this.graphData as IGraphData<T>;
        }

        public IEnumerable<NodeWrapper<T>> GetAllNodes()
        {
            foreach (var pair in nodeMapping)
            {
                yield return pair.Value;
            }
        }
    }
}
