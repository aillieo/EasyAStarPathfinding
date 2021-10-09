using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class AStar<T> : ISolver<T> where T : IGraphNode
    {
        public PathfindingState state { get; protected set; }

        private readonly List<T> result = new List<T>();

        internal readonly PathfindingContext<T> context;
        internal T startingNode;
        internal T endingNode;

        public AStar(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.context = new PathfindingContext<T>(graphData, algorithm);
            this.state = PathfindingState.Uninitialized;
        }

        public void CleanUp()
        {
            context.Reset();
            result.Clear();
            state = PathfindingState.Uninitialized;
        }

        public void Init()
        {
            this.state = PathfindingState.Initialized;
        }

        public void Init(T startingNode, T endingNode)
        {
            this.context.Reset();

            this.startingNode = startingNode;
            this.endingNode = endingNode;
            this.Init();
        }

        public PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;
                var startingNodeWrapper = context.GetOrCreateNode(this.startingNode);
                startingNodeWrapper.state = NodeState.Open;
                context.openList.Enqueue(startingNodeWrapper);
                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            NodeWrapper<T> first = context.openList.TryDequeue();
            if (first != null)
            {
                // 把周围点 加入open
                var neighbors = context.GetGraphData().CollectNeighbor(first.node);
                foreach (T p in neighbors)
                {
                    Collect(p, first);
                }

                first.state = NodeState.Closed;

                if (first.node.Equals(this.endingNode))
                {
                    state = PathfindingState.Found;
                    TraceBackForPath(first);
                    return state;
                }
            }
            else
            {
                state = PathfindingState.Failed;
                TraceBackForPath(null);
                return state;
            }

            return state;
        }

        protected bool Collect(T node, NodeWrapper<T> parentNode)
        {
            NodeWrapper<T> oldNodeWrapper = context.TryGetNode(node);
            if (oldNodeWrapper != null && oldNodeWrapper.state == NodeState.Closed)
            {
                return false;
            }

            bool changed = false;
            NodeWrapper<T> nodeWrapper = context.GetOrCreateNode(node);
            nodeWrapper.previous = parentNode;
            nodeWrapper.g = CalculateG(nodeWrapper);
            nodeWrapper.h = CalculateH(nodeWrapper);

            if (oldNodeWrapper == null)
            {
                changed = true;
                nodeWrapper.state = NodeState.Open;
                this.context.openList.Enqueue(nodeWrapper);
            }
            else if (nodeWrapper.g < oldNodeWrapper.g)
            {
                changed = true;
                oldNodeWrapper.g = nodeWrapper.g;
                this.context.openList.Update(oldNodeWrapper);
            }

            return changed;
        }

        protected virtual float CalculateG(NodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper.previous == null)
            {
                return 0f;
            }

            return nodeWrapper.previous.g + HeuristicFunc(nodeWrapper.node, nodeWrapper.previous.node) * (1 + nodeWrapper.node.cost);
        }

        protected virtual float CalculateH(NodeWrapper<T> nodeWrapper)
        {
            return HeuristicFunc(nodeWrapper.node, this.endingNode);
        }

        protected virtual float HeuristicFunc(T nodeFrom, T nodeTo)
        {
            return context.GetGraphData().DefaultHeuristicFunc(nodeFrom, nodeTo);
        }

        protected bool TraceBackForPath(INodeWrapper<T> endingNode)
        {
            result.Clear();
            if (endingNode == null)
            {
                return false;
            }

            NodeWrapper<T> node = endingNode as NodeWrapper<T>;
            while (node.previous != null)
            {
                result.Add(node.node);
                node = node.previous;
            }
            result.Add(node.node);
            return true;
        }

        public IEnumerable<T> GetResult()
        {
            foreach (var t in result)
            {
                yield return t;
            }
        }
    }
}
