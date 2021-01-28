using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.PathFinding
{
    public class PathFinder
    {
        private readonly PointNode.PointNodePool pool = PointNode.Pool();
        private readonly IGridDataProvider gridDataProvider;
        private readonly SortedSet<PointNode> openList;
        private readonly HashSet<PointNode> closed;
        private readonly PointNodeComparer comparer;
        private readonly NeighborCollectingFunc neighborCollectingFunc;

        private PointNode endingNode;

        public PathFinder(IGridDataProvider gridDataProvider)
            : this(gridDataProvider, null, null)
        {
        }

        public PathFinder(IGridDataProvider gridDataProvider, CostFunc costFunc)
            : this(gridDataProvider, costFunc, null)
        {
        }

        public PathFinder(IGridDataProvider gridDataProvider, NeighborCollectingFunc neighborCollectingFunc)
            : this(gridDataProvider, null, neighborCollectingFunc)
        {
        }

        public PathFinder(IGridDataProvider gridDataProvider, CostFunc costFunc, NeighborCollectingFunc neighborCollectingFunc)
        {
            this.gridDataProvider = gridDataProvider;
            if (costFunc == null)
            {
                costFunc = CostFuncPreset.DefaultCostFunc;
            }

            if (neighborCollectingFunc == null)
            {
                neighborCollectingFunc = NeighborCollectingFuncPreset.DefaultNeighborCollectingFunc;
            }

            this.neighborCollectingFunc = neighborCollectingFunc;
            this.comparer = new PointNodeComparer(costFunc);
            this.openList = new SortedSet<PointNode>(comparer);
            this.closed = new HashSet<PointNode>(new PointNodeEqualityComparer());
        }

        private void Init(Point startPoint, Point endPoint)
        {
            comparer.Init(startPoint, endPoint);
            openList.Clear();
            closed.Clear();
            endingNode = null;
        }

        private void CollectNeighbors(PointNode pointNode)
        {
            // 把周围点 加入open
            var neighbors = neighborCollectingFunc(pointNode.point, gridDataProvider);
            foreach (Point p in neighbors)
            {
                PointNode newNode = pool.GetPointNode(p, pointNode);
                if (closed.Contains(newNode))
                {
                    pool.Recycle(newNode);
                    continue;
                }

                if (!openList.Add(newNode))
                {
                    pool.Recycle(newNode);
                }
            }
        }

        private void CleanUp()
        {
            foreach (var p in openList)
            {
                pool.Recycle(p);
            }
            foreach (var p in closed)
            {
                pool.Recycle(p);
            }
            openList.Clear();
            closed.Clear();
        }

        public IEnumerable<Point> FindPath(Point startPoint, Point endPoint)
        {
            Init(startPoint, endPoint);

            openList.Add(pool.GetPointNode(startPoint, null));

            while (openList.Count > 0)
            {
                // 取第一个
                var first = openList.Min;

                CollectNeighbors(first);

                // 将first移入close
                openList.Remove(first);
                closed.Add(first);

                if (first.point == endPoint)
                {
                    // 找到终点了
                    endingNode = first;
                    break;
                }
            }

            List<Point> points = new List<Point>();
            TraceBackForPath(points);

            CleanUp();

            return points;
        }

        private bool TraceBackForPath(List<Point> toFill)
        {
            if (endingNode == null)
            {
                return false;
            }

            var node = endingNode;
            while (node.previous != null)
            {
                toFill.Add(node.point);
                node = node.previous;
            }
            toFill.Add(node.point);

            return true;
        }

        public Task<IEnumerable<Point>> FindPathAsync(Point startPoint, Point endPoint)
        {
            TaskCompletionSource<IEnumerable<Point>> tsc = new TaskCompletionSource<IEnumerable<Point>>();
            ThreadPool.QueueUserWorkItem(_ => {
                var points = this.FindPath(startPoint, endPoint);
                tsc.SetResult(points);
            });
            return tsc.Task;
        }

        public IEnumerator FindPathInCoroutine(Point startPoint, Point endPoint, UnityEngine.YieldInstruction yieldInstruction, PointChangedFunc pointChanged)
        {
            Init(startPoint, endPoint);

            openList.Add(pool.GetPointNode(startPoint, null));

            while (openList.Count > 0)
            {
                yield return yieldInstruction;

                var first = openList.Min;

                CollectNeighbors(first);

                openList.Remove(first);
                closed.Add(first);

                if (first.point == endPoint)
                {
                    endingNode = first;
                    break;
                }
            }

            List<Point> points = new List<Point>();
            TraceBackForPath(points);

            CleanUp();
        }
    }
}
