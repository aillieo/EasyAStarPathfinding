using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public abstract class AStar<T> : ISolver where T : IGraphNode
    {
        public PathfindingState state { get; private set; }

        private readonly List<T> result = new List<T>();

        internal readonly PathfindingContext<T> context;

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

        public void Init(T startPoint, T endPoint)
        {
            this.context.Reset();
            this.context.startingNode = startPoint;
            this.context.endingNode = endPoint;
            this.Init();
        }

        public PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;
                var startNode = context.GetNode(context.startingNode, null);
                context.openList.Enqueue(startNode);
                context.openSet.Add(context.startingNode, startNode);
                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            if (context.openList.Count > 0)
            {
                var first = context.openList.Dequeue();
                context.openSet.Remove(first.node);

                // 把周围点 加入open
                var neighbors = context.graphData.CollectNeighbor(first.node);
                foreach (T p in neighbors)
                {
                    Collect(p, first);
                }

                context.closedSet.Add(first.node, first);

                if (context.IsEndingNode(first.node))
                {
                    context.endingPointer = first;
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

        private bool Collect(T node, NodePointer<T> parentNode)
        {
            if (this.context.closedSet.ContainsKey(node))
            {
                return false;
            }

            bool changed = false;
            NodePointer<T> nodePointer = context.GetNode(node, parentNode);
            nodePointer.g = GetG(nodePointer);
            nodePointer.h = GetH(nodePointer);

            if (!this.context.openSet.ContainsKey(node))
            {
                changed = true;
            }
            else
            {
                NodePointer<T> oldNodePointer = this.context.openSet[node];
                if (nodePointer.g < oldNodePointer.g)
                {
                    this.context.openList.Remove(oldNodePointer);
                    this.context.openSet.Remove(node);
                    changed = true;
                }
            }

            if (changed)
            {
                this.context.openList.Enqueue(nodePointer);
                this.context.openSet.Add(node, nodePointer);
            }

            return changed;
        }

        protected virtual float GetG(NodePointer<T> nodePointer)
        {
            if (nodePointer.previous == null)
            {
                return 0f;
            }

            return nodePointer.previous.g + HeuristicFunc(nodePointer.node, nodePointer.previous.node) * nodePointer.node.cost;
        }

        protected virtual float GetH(NodePointer<T> nodePointer)
        {
            return HeuristicFunc(nodePointer.node, context.endingNode);
        }

        protected abstract float HeuristicFunc(T nodeFrom, T nodeTo);

        private bool TraceBackForPath()
        {
            result.Clear();
            if (this.context.endingPointer == null)
            {
                return false;
            }

            var node = this.context.endingPointer;
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
