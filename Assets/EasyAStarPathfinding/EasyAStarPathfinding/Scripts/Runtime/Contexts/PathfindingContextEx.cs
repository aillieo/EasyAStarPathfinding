using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContextEx<T> : IPathfindingContext<T, NodeWrapperEx<T>> where T : IGraphNode
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

        public NodeWrapperEx<T> GetOrCreateNode(T node)
        {
            if (!nodeMapping.TryGetValue(node, out NodeWrapperEx<T> nodeWrapperEx))
            {
                nodeWrapperEx = new NodeWrapperEx<T>(node);
                nodeMapping.Add(node, nodeWrapperEx);
            }

            return nodeWrapperEx;
        }

        public void Reset()
        {
            this.nodeMapping.Clear();
            this.openList.Clear();
        }

        public NodeWrapperEx<T> TryGetOpenNode(T nodeData)
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

        public NodeWrapperEx<T> TryGetClosedNode(T nodeData)
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

        public void AddToOpen(T nodeData, INodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper is NodeWrapperEx<T> nodeWrapperEx)
            {
                nodeWrapperEx.state = NodeState.Open;
                openList.Enqueue(nodeWrapperEx);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IEnumerable<NodeWrapperEx<T>> GetAllNodes()
        {
            foreach (var pair in nodeMapping)
            {
                yield return pair.Value;
            }
        }

        public NodeWrapperEx<T> TryGetFrontier()
        {
            if (openList.Count > 0)
            {
                return openList.Dequeue();
            }

            return null;
        }

        public void UpdateFrontier(INodeWrapper<T> nodeWrapper)
        {
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
