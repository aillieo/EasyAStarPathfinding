using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class Pathfinder
    {
        internal protected PathfindingContext context;

        protected readonly IGridData gridDataProvider;

        public Pathfinder(IGridData gridDataProvider)
            : this(gridDataProvider, null)
        {
        }

        public Pathfinder(IGridData gridDataProvider, HeuristicFunc costFunc)
        {
            this.gridDataProvider = gridDataProvider;
            if (costFunc == null)
            {
                costFunc = HeuristicFuncPreset.DefaultCostFunc;
            }
            this.context = new PathfindingContext();
        }

        protected void Init(Point startPoint, Point endPoint)
        {
            this.context.Reset();
            this.context.endingPoint = endPoint;
        }

        protected bool Collect(Point point, PointNode parentNode = null)
        {
            if (this.context.closedSet.Contains(point))
            {
                return false;
            }

            if(this.context.openSet.Contains(point))
            {
                // todo 如果<=之前的g 需要更新g
                return false;
            }

            PointNode newNode = context.pool.GetPointNode(point, parentNode);
            if (parentNode != null)
            {
                newNode.g = parentNode.g + HeuristicFuncPreset.DefaultCostFunc(point, parentNode.point);
                newNode.h = HeuristicFuncPreset.DefaultCostFunc(point, this.context.endingPoint);
            }
            this.context.openList.Enqueue(newNode);
            this.context.openSet.Add(point);

            return true;
        }

        protected void CleanUp()
        {
            context.Reset();
        }

        public IEnumerable<Point> FindPath(Point startPoint, Point endPoint)
        {
            Init(startPoint, endPoint);

            var startNode = context.pool.GetPointNode(startPoint, null);
            this.context.openList.Enqueue(startNode);
            this.context.openSet.Add(startPoint);

            while (this.context.openList.Count > 0)
            {
                // 取第一个
                var first = this.context.openList.Dequeue();
                context.openSet.Remove(first.point);

                // 把周围点 加入open
                var neighbors = gridDataProvider.CollectNeighbor(first.point);
                foreach (Point p in neighbors)
                {
                    Collect(p, first);
                }

                // 将first移入close
                this.context.closedSet.Add(first.point);

                if (first.point == endPoint)
                {
                    // 找到终点了
                    this.context.endingNode = first;
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
            if (this.context.endingNode == null)
            {
                return false;
            }

            var node = this.context.endingNode;
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
