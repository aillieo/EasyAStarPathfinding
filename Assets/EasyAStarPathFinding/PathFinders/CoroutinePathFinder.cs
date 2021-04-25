using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.PathFinding
{
    public class CoroutinePathFinder : PathFinder
    {
        public CoroutinePathFinder(IGridDataProvider gridDataProvider)
            : base(gridDataProvider)
        {
        }

        public CoroutinePathFinder(IGridDataProvider gridDataProvider, CostFunc costFunc)
            : base(gridDataProvider, costFunc)
        {
        }

        public CoroutinePathFinder(IGridDataProvider gridDataProvider, CostFunc costFunc, NeighborCollectingFunc neighborCollectingFunc)
            : base(gridDataProvider, costFunc, neighborCollectingFunc)
        {
        }

        public IEnumerator FindPathInCoroutine(Point startPoint, Point endPoint, UnityEngine.YieldInstruction yieldInstruction, PointChangedFunc pointChanged)
        {
            Init(startPoint, endPoint);

            openList.Enqueue(pool.GetPointNode(startPoint, null));
            pointChanged?.Invoke(new PointChangeInfo(startPoint, PointChangeFlag.Add | PointChangeFlag.OpenList));

            while (openList.Count > 0)
            {
                yield return yieldInstruction;

                var first = openList.Dequeue();
                pointChanged?.Invoke(new PointChangeInfo(first.point, PointChangeFlag.Remove | PointChangeFlag.OpenList));

                // 把周围点 加入open
                var neighbors = neighborCollectingFunc(first.point, gridDataProvider);
                foreach (Point p in neighbors)
                {
                    if (Collect(p, first))
                    {
                        pointChanged?.Invoke(new PointChangeInfo(p, PointChangeFlag.Add | PointChangeFlag.OpenList));
                    }
                }

                closed.Add(first);
                pointChanged?.Invoke(new PointChangeInfo(first.point, PointChangeFlag.Add | PointChangeFlag.ClosedList));

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
