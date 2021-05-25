using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.PathFinding
{
    public class PathFinder
    {
        internal readonly PointNode.PointNodePool pool = PointNode.Pool();
        protected readonly IGridDataProvider gridDataProvider;
        protected readonly UniquePriorityQueue<PointNode> openList;
        protected readonly HashSet<PointNode> closed;
        internal readonly PointNodeComparer comparer;
        protected readonly NeighborCollectingFunc neighborCollectingFunc;

        protected PointNode endingNode;
        protected Point endingPoint;

        public PathFinder(IGridDataProvider gridDataProvider)
            : this(gridDataProvider, null, null)
        {
        }

        public PathFinder(IGridDataProvider gridDataProvider, HeuristicFunc costFunc)
            : this(gridDataProvider, costFunc, null)
        {
        }

        public PathFinder(IGridDataProvider gridDataProvider, NeighborCollectingFunc neighborCollectingFunc)
            : this(gridDataProvider, null, neighborCollectingFunc)
        {
        }

        public PathFinder(IGridDataProvider gridDataProvider, HeuristicFunc costFunc, NeighborCollectingFunc neighborCollectingFunc)
        {
            this.gridDataProvider = gridDataProvider;
            if (costFunc == null)
            {
                costFunc = HeuristicFuncPreset.DefaultCostFunc;
            }

            if (neighborCollectingFunc == null)
            {
                neighborCollectingFunc = NeighborCollectingFuncPreset.DefaultNeighborCollectingFunc;
            }

            this.neighborCollectingFunc = neighborCollectingFunc;
            this.comparer = new PointNodeComparer(costFunc);
            this.openList = new UniquePriorityQueue<PointNode>(this.comparer);
            this.closed = new HashSet<PointNode>(PointNodeEqualityComparer.instance);
        }

        protected void Init(Point startPoint, Point endPoint)
        {
            this.endingPoint = endPoint;
            comparer.Init(startPoint, endPoint);
            openList.Clear();
            closed.Clear();
            endingNode = null;
        }

        protected bool Collect(Point point, PointNode parentNode = null)
        {
            PointNode newNode = PointNode.Dummy(point);
            if (closed.Contains(newNode))
            {
                return false;
            }

            PointNode newNode1 = pool.GetPointNode(point, parentNode);
            newNode1.g = parentNode != null ? parentNode.g : 0f;
            newNode1.g += HeuristicFuncPreset.ManhattanDist(point, endingPoint);
            if (!openList.Enqueue(newNode1))
            {
                pool.Recycle(newNode1);
                return false;
            }

            return true;
        }

        protected void CleanUp()
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

            openList.Enqueue(pool.GetPointNode(startPoint, null));

            while (openList.Count > 0)
            {
                // 取第一个
                var first = openList.Dequeue();

                // 把周围点 加入open
                var neighbors = neighborCollectingFunc(first.point, gridDataProvider);
                foreach (Point p in neighbors)
                {
                    Collect(p, first);
                }

                // 将first移入close
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

        protected bool TraceBackForPath(List<Point> toFill)
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
    }
}
