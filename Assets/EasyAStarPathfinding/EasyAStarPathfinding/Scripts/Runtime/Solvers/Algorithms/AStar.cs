using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public abstract class AStar : ISolver
    {
        public PathfindingState state { get; private set; }

        private readonly List<Grid> result = new List<Grid>();

        internal readonly PathfindingContext context;

        public AStar(IGridMapData gridData, Algorithms algorithm)
        {
            this.context = new PathfindingContext(gridData, algorithm);
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

        public void Init(Grid startPoint, Grid endPoint)
        {
            this.context.Reset();
            this.context.startPoint = startPoint;
            this.context.endingPoint = endPoint;
            this.Init();
        }

        public PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;
                var startNode = context.GetPointNode(context.startPoint, null);
                context.openList.Enqueue(startNode);
                context.openSet.Add(context.startPoint);
                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            if (context.openList.Count > 0)
            {
                var first = context.openList.Dequeue();
                context.openSet.Remove(first.grid);

                // 把周围点 加入open
                var neighbors = context.graphData.CollectNeighbor(first.grid);
                foreach (Grid p in neighbors)
                {
                    Collect(p, first);
                }

                context.closedSet.Add(first.grid);

                if (first.grid == context.endingPoint)
                {
                    context.endingNode = first;
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

        private bool Collect(Grid grid, GridNode parentNode)
        {
            if (this.context.closedSet.Contains(grid))
            {
                return false;
            }

            if (this.context.openSet.Contains(grid))
            {
                // todo 如果<=之前的g 需要更新g
                return false;
            }

            GridNode newNode = context.GetPointNode(grid, parentNode);
            newNode.g = parentNode.g + HeuristicFuncPreset.DefaultCostFunc(grid, parentNode.grid);
            newNode.h = HeuristicFuncPreset.DefaultCostFunc(grid, this.context.endingPoint);
            this.context.openList.Enqueue(newNode);
            this.context.openSet.Add(grid);

            return true;
        }

        private bool TraceBackForPath()
        {
            result.Clear();
            if (this.context.endingNode == null)
            {
                return false;
            }

            var node = this.context.endingNode;
            while (node.previous != null)
            {
                result.Add(node.grid);
                node = node.previous;
            }
            result.Add(node.grid);
            return true;
        }

        public void GetResult(List<Grid> toFill)
        {
            toFill.Clear();
            toFill.AddRange(result);
        }
    }
}
