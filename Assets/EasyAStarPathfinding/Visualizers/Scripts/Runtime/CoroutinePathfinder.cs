using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class CoroutinePathfinder : Pathfinder
    {
        public CoroutinePathfinder(IGridData gridDataProvider)
            : base(gridDataProvider)
        {
        }
        public CoroutinePathfinder(IGridData gridDataProvider, HeuristicFunc costFunc)
            : base(gridDataProvider, costFunc)
        {
        }

        public IEnumerator FindPathInCoroutine(Point startPoint, Point endPoint, UnityEngine.YieldInstruction yieldInstruction)
        {
            Init(startPoint, endPoint);

            var startNode = context.pool.GetPointNode(startPoint, null);
            context.openList.Enqueue(startNode);
            context.openSet.Add(startPoint);

            while (context.openList.Count > 0)
            {
                var first = context.openList.Dequeue();
                context.openSet.Remove(first.point);

                yield return yieldInstruction;

                // 把周围点 加入open
                var neighbors = gridDataProvider.CollectNeighbor(first.point);
                foreach (Point p in neighbors)
                {
                    Collect(p, first);
                }

                context.closedSet.Add(first.point);

                if (first.point == endPoint)
                {
                    context.endingNode = first;
                    break;
                }
            }

            List<Point> points = new List<Point>();
            TraceBackForPath(points);

            CleanUp();
        }
    }
}
