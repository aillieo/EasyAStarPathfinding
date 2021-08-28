using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext<T> : IPathfindingContext<T> where T : IGraphNode
    {
        private readonly UniquePriorityQueue<NodeWrapper<T>> openList;
        private readonly Dictionary<T, NodeWrapper<T>> nodeMapping;
        //internal T startingNode;
        //internal T endingNode;
        private readonly IGraphData<T> graphData;
        private readonly Algorithms algorithm;

        internal PathfindingContext(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new UniquePriorityQueue<NodeWrapper<T>>();
            this.nodeMapping = new Dictionary<T, NodeWrapper<T>>();
        }

        public NodeWrapper<T> CreateNewNode(T node, NodeWrapper<T> previous)
        {
            return new NodeWrapper<T>(node, previous);
        }

        public void Reset()
        {
            this.nodeMapping.Clear();
            this.openList.Clear();
        }

        public NodeWrapper<T> TryGetOpenNode(T nodeData)
        {
            NodeWrapper<T> node = null;
            if (this.nodeMapping.TryGetValue(nodeData, out node))
            {
                if (node.state == NodeState.Open)
                {
                    return node;
                }
            }

            return null;
        }

        public NodeWrapper<T> TryGetClosedNode(T nodeData)
        {
            NodeWrapper<T> node = null;
            if (this.nodeMapping.TryGetValue(nodeData, out node))
            {
                if (node.state == NodeState.Closed)
                {
                    return node;
                }
            }

            return null;
        }

        public IGraphData<T> GetGraphData()
        {
            return this.graphData;
        }

        public bool RemoveFromMapping(T node)
        {
            return this.nodeMapping.Remove(node);
        }

        public void AddToOpen(T nodeData, NodeWrapper<T> nodeWrapper)
        {
            nodeWrapper.state = NodeState.Open;
            openList.Enqueue(nodeWrapper);
            nodeMapping.Add(nodeData, nodeWrapper);
        }

        public void AddToClosed(T nodeData, NodeWrapper<T> nodeWrapper)
        {
            nodeWrapper.state = NodeState.Closed;
            nodeMapping.Add(nodeData, nodeWrapper);
        }

        public IEnumerable<NodeWrapper<T>> GetAllNodes()
        {
            foreach (var pair in nodeMapping)
            {
                yield return pair.Value;
            }
        }

        public NodeWrapper<T> TryGetFrontier()
        {
            if (openList.Count > 0)
            {
                return openList.Dequeue();
            }

            return null;
        }

        public void UpdateFrontier(NodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper == null)
            {
                return;
            }

            if (openList.Remove(nodeWrapper))
            {
                openList.Enqueue(nodeWrapper);
            }
        }
    }
}
