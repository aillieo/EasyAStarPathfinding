using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.PathFinding
{
    public class PathFinder
    {
        public delegate int CostFunc(Point point, Point startPoint, Point endPoint);

        private SortedSet<PointNode> openList;// = new SortedSet<PathPointNode>();
        private HashSet<PointNode> closed;// = new HashSet<PathPointNode>();
        private PointNode endingNode;

        private IGridDataProvider mapData;
        private Comparer comparer;

        private Stack<PointNode> nodePool = new Stack<PointNode>();

        private class Comparer : IComparer<PointNode>
        {
            private CostFunc costFunc;
            public Point startPoint;
            public Point endPoint;

            public Comparer(CostFunc costFunc)
            {
                this.costFunc = costFunc;
            }

            public int Compare(PointNode lhs, PointNode rhs)
            {
                int cost1 = costFunc(lhs.point, startPoint, endPoint);
                int cost2 = costFunc(rhs.point, startPoint, endPoint);
                if (cost1 != cost2)
                {
                    return cost1 - cost2;
                }
                return lhs.point.CompareTo(rhs.point);
            }
        }

        private static int DefaultCostFunc(Point current, Point startPoint, Point endPoint)
        {
            return Math.Abs(startPoint.x - current.x) +
                Math.Abs(startPoint.y - current.y) +
                Math.Abs(endPoint.x - current.x) +
                Math.Abs(endPoint.y - current.y);
        }

        public PathFinder(IGridDataProvider mapData, CostFunc costFunc = null)
        {
            this.mapData = mapData;
            if (costFunc == null)
            {
                costFunc = DefaultCostFunc;
            }

            comparer = new Comparer(costFunc);
            this.openList = new SortedSet<PointNode>(comparer);
            this.closed = new HashSet<PointNode>();
        }

        private PointNode GetPointNode(Point point, PointNode parent)
        {
            PointNode newNode;
            if (nodePool.Count > 0)
            {
                newNode = nodePool.Pop();
                newNode.point = point;
                newNode.previous = parent;
            }
            else
            {
                newNode = new PointNode { point = point, previous = parent };
            }
            return newNode;
        }

        private void Recycle(PointNode node)
        {
            nodePool.Push(node);
        }

        public IEnumerable<Point> FindPath(Point startPoint, Point endPoint)
        {
            this.comparer.startPoint = startPoint;
            this.comparer.endPoint = endPoint;
            openList.Clear();
            closed.Clear();
            endingNode = null;

            openList.Add(GetPointNode(startPoint, null));

            int safe = 0;
            while (true)
            {
                if (safe++ > 10000000)
                {
                    UnityEngine.Debug.LogError($"safe = {safe}");
                    break;
                }

                if (openList.Count == 0)
                {
                    break;
                }

                // 取第一个
                var first = openList.First();

                // 把周围点 加入open
                for (int i = -1; i <= 1; ++i)
                {
                    for (int j = -1; j <= 1; ++j)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }

                        if (i != 0 && j != 0)
                        {
                            // 禁止斜着走
                            continue;
                        }

                        int x = first.point.x + i;
                        int y = first.point.y + j;

                        // Debug.LogError($" TESTING  x={x} y= {y}");

                        if (!mapData.Passable(x, y))
                        {
                            continue;
                        }

                        Point p = new Point() { x = x, y = y };
                        PointNode node = GetPointNode(p, first);
                        if (closed.Contains(node))
                        {
                            Recycle(node);
                            continue;
                        }

                        if(!openList.Contains(node))
                        {
                            openList.Add(node);
                        }
                        else
                        {
                            Recycle(node);
                        }
                    }
                }

                // 将first移入close
                openList.Remove(first);
                closed.Add(first);

                if (first.point.x == endPoint.x && first.point.y == endPoint.y)
                {
                    // 找到终点了
                    endingNode = first;
                    break;
                }

                //Debug.LogError($"safe = {safe}  openList = {openList.Count} closed = {closed.Count}  first = ({first.point.x},{first.point.y})");
            }

            List<Point> points = new List<Point>();
            if (endingNode != null)
            {
                var ppp = endingNode;
                while (ppp.previous != null)
                {
                    points.Add(ppp.point);
                    ppp = ppp.previous;
                }
                points.Add(ppp.point);
            }

            foreach (var p in openList)
            {
                Recycle(p);
            }
            foreach (var p in closed)
            {
                Recycle(p);
            }
            openList.Clear();
            closed.Clear();

            return points;
        }

    }

}
