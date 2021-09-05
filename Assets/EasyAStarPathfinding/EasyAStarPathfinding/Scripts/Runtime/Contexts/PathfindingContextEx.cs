using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContextEx<T> : IPathfindingContext<T> where T : IGraphNode
    {
        private readonly UniquePriorityQueue<NodeWrapperEx<T>> openList;
        private readonly Dictionary<T, NodeWrapperEx<T>> nodeMapping;
        //internal T startingNode;
        //internal T endingNode;
        private readonly IGraphData<T> graphData;
        private readonly Algorithms algorithm;

        internal PathfindingContextEx(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.graphData = graphData;
            this.algorithm = algorithm;
            this.openList = new UniquePriorityQueue<NodeWrapperEx<T>>();
            this.nodeMapping = new Dictionary<T, NodeWrapperEx<T>>();
        }

        public NodeWrapper<T> CreateNewNode(T node, NodeWrapper<T> previous)
        {
            return new NodeWrapperEx<T>(node, previous);
        }

        public void Reset()
        {
            this.nodeMapping.Clear();
            this.openList.Clear();
        }

        public NodeWrapper<T> TryGetOpenNode(T nodeData)
        {
            NodeWrapperEx<T> node = null;
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
            NodeWrapperEx<T> node = null;
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
            if (nodeWrapper is NodeWrapperEx<T> nodeWrapperEx)
            {
                openList.Enqueue(nodeWrapperEx);
                nodeMapping.Add(nodeData, nodeWrapperEx);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void AddToClosed(T nodeData, NodeWrapper<T> nodeWrapper)
        {
            nodeWrapper.state = NodeState.Closed;
            if (nodeWrapper is NodeWrapperEx<T> nodeWrapperEx)
            {
                nodeMapping.Add(nodeData, nodeWrapperEx);
            }
            else
            {
                throw new InvalidOperationException();
            }
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

            if (nodeWrapper is NodeWrapperEx<T> nodeWrapperEx)
            {
                if (openList.Remove(nodeWrapperEx))
                {
                    openList.Enqueue(nodeWrapperEx);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
