using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    public class PathfindingContext<T> : IPathfindingContext<T, NodeWrapper<T>> where T : IGraphNode
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

        public NodeWrapper<T> GetOrCreateNode(T node)
        {
            if (!nodeMapping.TryGetValue(node, out NodeWrapper<T> nodeWrapper))
            {
                nodeWrapper = new NodeWrapper<T>(node);
                nodeMapping.Add(node, nodeWrapper);
            }

            return nodeWrapper;
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

        public void AddToOpen(T nodeData, INodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper is NodeWrapper<T> nodeWrapperT)
            {
                nodeWrapperT.state = NodeState.Open;
                openList.Enqueue(nodeWrapperT);
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

        public void UpdateFrontier(INodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper is NodeWrapper<T> nodeWrapperT)
            {
                if (openList.Remove(nodeWrapperT))
                {
                    openList.Enqueue(nodeWrapperT);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
