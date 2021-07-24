using System;
using System.Collections.Generic;

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
                var startNode = context.GetPointNode(context.startingNode, null);
                context.openList.Enqueue(startNode);
                context.openSet.Add(context.startingNode);
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

                context.closedSet.Add(first.node);

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
            if (this.context.closedSet.Contains(node))
            {
                return false;
            }

            if (this.context.openSet.Contains(node))
            {
                // todo 如果<=之前的g 需要更新g
                return false;
            }

            NodePointer<T> newNode = context.GetPointNode(node, parentNode);
            newNode.g = parentNode.g + HeuristicFuncPreset.DefaultCostFunc(node as Grid, parentNode.node as Grid);
            newNode.h = HeuristicFuncPreset.DefaultCostFunc(node as Grid, this.context.endingNode as Grid);
            this.context.openList.Enqueue(newNode);
            this.context.openSet.Add(node);

            return true;
        }

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
