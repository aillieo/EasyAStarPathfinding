using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public abstract class AStar<T> : ISolver<T> where T : IGraphNode
    {
        public PathfindingState state { get; protected set; }

        private readonly List<T> result = new List<T>();

        internal readonly IPathfindingContext<T, INodeWrapper<T>> context;
        internal NodeWrapper<T> endingNodeWrapper;
        internal T startingNode;
        internal T endingNode;

        public AStar(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.context = ContextCreator<T>.CreateContext(graphData, algorithm);
            this.state = PathfindingState.Uninitialized;
        }

        public virtual void CleanUp()
        {
            context.Reset();
            result.Clear();
            state = PathfindingState.Uninitialized;
        }

        public virtual void Init()
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

        public virtual PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;
                var startNode = context.GetOrCreateNode(this.startingNode);
                context.AddToOpen(this.startingNode, startNode);
                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            NodeWrapper<T> first = context.TryGetFrontier() as NodeWrapper<T>;
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
                    this.endingNodeWrapper = first;
                    state = PathfindingState.Found;
                    TraceBackForPath();
                    return state;
                }
            }
            else
            {
                state = PathfindingState.Failed;
                TraceBackForPath();
                return state;
            }

            return state;
        }

        protected virtual bool Collect(T node, NodeWrapper<T> parentNode)
        {
            if (this.context.TryGetClosedNode(node) != null)
            {
                return false;
            }

            bool changed = false;
            NodeWrapper<T> nodeWrapper = context.GetOrCreateNode(node) as NodeWrapper<T>;
            nodeWrapper.previous = parentNode;
            nodeWrapper.g = CalculateG(nodeWrapper);
            nodeWrapper.h = CalculateH(nodeWrapper);

            NodeWrapper<T> oldNodeWrapper = context.TryGetOpenNode(node) as NodeWrapper<T>;
            if (oldNodeWrapper == null)
            {
                changed = true;
                this.context.AddToOpen(node, nodeWrapper);
            }
            else if (nodeWrapper.g < oldNodeWrapper.g)
            {
                changed = true;
                oldNodeWrapper.g = nodeWrapper.g;
                this.context.UpdateFrontier(oldNodeWrapper);
            }

            return changed;
        }

        protected virtual float CalculateG(NodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper.previous == null)
            {
                return 0f;
            }

            return nodeWrapper.previous.g + HeuristicFunc(nodeWrapper.node, nodeWrapper.previous.node) * nodeWrapper.node.cost;
        }

        protected virtual float CalculateH(NodeWrapper<T> nodeWrapper)
        {
            return HeuristicFunc(nodeWrapper.node, this.endingNode);
        }

        protected abstract float HeuristicFunc(T nodeFrom, T nodeTo);

        protected bool TraceBackForPath()
        {
            result.Clear();
            if (this.endingNodeWrapper == null)
            {
                return false;
            }

            NodeWrapper<T> node = this.endingNodeWrapper;
            while (node.previous != null)
            {
                result.Add(node.node);
                node = node.previous;
            }
            result.Add(node.node);
            return true;
        }

        public void GetResult(List<T> toFill)
        {
            toFill.Clear();
            toFill.AddRange(result);
        }
    }
}
