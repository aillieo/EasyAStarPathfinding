using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.PathFinding
{
    public class AsyncPathFinder : PathFinder
    {
        public AsyncPathFinder(IGridDataProvider gridDataProvider)
            : base(gridDataProvider)
        {
        }

        public AsyncPathFinder(IGridDataProvider gridDataProvider, CostFunc costFunc)
            : base(gridDataProvider, costFunc)
        {
        }

        public AsyncPathFinder(IGridDataProvider gridDataProvider, CostFunc costFunc, NeighborCollectingFunc neighborCollectingFunc)
            : base(gridDataProvider, costFunc, neighborCollectingFunc)
        {
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
    }
}
