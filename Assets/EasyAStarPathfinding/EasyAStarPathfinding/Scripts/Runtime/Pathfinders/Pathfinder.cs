using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.Pathfinding
{
    public class Pathfinder
    {
        public PathfindingState state { get; private set; }

        internal readonly PathfindingContext context;
        internal readonly ISolver solver;
        private readonly List<Point> result = new List<Point>();

        public Pathfinder(IGraphData graphData, Algorithms algorithm = Algorithms.AStar)
        {
            this.context = new PathfindingContext(graphData as IGridData, algorithm);
            this.solver = Solvers.Create(graphData, algorithm);
            this.state = PathfindingState.Uninitialized;
        }

        public void Init(Point startPoint, Point endPoint)
        {
            this.context.Reset();
            this.context.startPoint = startPoint;
            this.context.endingPoint = endPoint;
            this.state = PathfindingState.Initialized;
        }

        public Point[] FindPath(Point startPoint, Point endPoint)
        {
            Init(startPoint, endPoint);
            while (true)
            {
                if (state == PathfindingState.Found || state == PathfindingState.Failed)
                {
                    break;
                }

                FindPath();
            }

            return result.ToArray();
        }

        public Task<Point[]> FindPathAsync(Point startPoint, Point endPoint)
        {
            TaskCompletionSource<Point[]> tsc = new TaskCompletionSource<Point[]>();
            ThreadPool.QueueUserWorkItem(_ => {
                var points = this.FindPath(startPoint, endPoint);
                tsc.SetResult(points);
            });
            return tsc.Task;
        }

        public PathfindingState FindPath()
        {
            switch (state)
            {
                case PathfindingState.Uninitialized:
                    throw new Exception("Uninitialized PathfindingContext");
                case PathfindingState.Initialized:
                    FirstStep();
                    break;
                case PathfindingState.Finding:
                    Step();
                    break;
                case PathfindingState.Found:
                case PathfindingState.Failed:
                    break;
            }

            return this.state;
        }

        private void FirstStep()
        {
            state = PathfindingState.Finding;
            var startNode = context.GetPointNode(context.startPoint, null);
            context.openList.Enqueue(startNode);
            context.openSet.Add(context.startPoint);
            return;
        }

        private void Step()
        {
            if (context.openList.Count > 0)
            {
                var first = context.openList.Dequeue();
                context.openSet.Remove(first.point);

                // 把周围点 加入open
                var neighbors = context.graphData.CollectNeighbor(first.point);
                foreach (Point p in neighbors)
                {
                    Collect(p, first);
                }

                context.closedSet.Add(first.point);

                if (first.point == context.endingPoint)
                {
                    context.endingNode = first;
                    state = PathfindingState.Found;
                    TraceBackForPath();
                    return;
                }
            }
            else
            {
                state = PathfindingState.Failed;
                TraceBackForPath();
                return;
            }
        }

        private bool Collect(Point point, PointNode parentNode)
        {
            if (this.context.closedSet.Contains(point))
            {
                return false;
            }

            if (this.context.openSet.Contains(point))
            {
                // todo 如果<=之前的g 需要更新g
                return false;
            }

            PointNode newNode = context.GetPointNode(point, parentNode);
            newNode.g = parentNode.g + HeuristicFuncPreset.DefaultCostFunc(point, parentNode.point);
            newNode.h = HeuristicFuncPreset.DefaultCostFunc(point, this.context.endingPoint);
            this.context.openList.Enqueue(newNode);
            this.context.openSet.Add(point);

            return true;
        }

        public virtual void CleanUp()
        {
            context.Reset();
            result.Clear();
            state = PathfindingState.Uninitialized;
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
                result.Add(node.point);
                node = node.previous;
            }
            result.Add(node.point);
            return true;
        }

        public void GetResult(List<Point> toFill)
        {
            toFill.Clear();
            toFill.AddRange(result);
        }

        public Point[] GetResult()
        {
            return result.ToArray();
        }
    }
}
