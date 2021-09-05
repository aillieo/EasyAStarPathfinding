using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public abstract class AStar<T> : ISolver<T> where T : IGraphNode
    {
        public PathfindingState state { get; private set; }

        private readonly List<T> result = new List<T>();

        internal readonly IPathfindingContext<T> context;
        internal NodeWrapper<T> endingNodeWrapper;
        internal T startingNode;
        internal T endingNode;

        public AStar(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.context = ContextCreator<T>.CreateContext(graphData, algorithm);
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
            //this.context.startingNode = this.startingNode;
            //this.context.endingNode = this.endingNode;
            this.Init();
        }

        public PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;
                var startNode = context.CreateNewNode(this.startingNode, null);
                context.AddToOpen(this.startingNode, startNode);
                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            NodeWrapper<T> first = context.TryGetFrontier();
            if (first != null)
            {
                context.RemoveFromMapping(first.node);

                // 把周围点 加入open
                var neighbors = context.GetGraphData().CollectNeighbor(first.node);
                foreach (T p in neighbors)
                {
                    Collect(p, first);
                }

                context.AddToClosed(first.node, first);

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

        private bool Collect(T node, NodeWrapper<T> parentNode)
        {
            if (this.context.TryGetClosedNode(node) != null)
            {
                return false;
            }

            bool changed = false;
            NodeWrapper<T> nodeWrapper = context.CreateNewNode(node, parentNode);
            nodeWrapper.g = CalculateG(nodeWrapper);
            nodeWrapper.h = CalculateH(nodeWrapper);

            NodeWrapper<T> oldNodeWrapper = context.TryGetOpenNode(node);
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

        private bool TraceBackForPath()
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
