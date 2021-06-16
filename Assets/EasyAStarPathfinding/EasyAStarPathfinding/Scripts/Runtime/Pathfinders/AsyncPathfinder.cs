using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.Pathfinding
{
    public class AsyncPathfinder : Pathfinder
    {
        public AsyncPathfinder(IGridData gridDataProvider, Algorithms algorithm = Algorithms.AStar)
            : base(gridDataProvider, algorithm)
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
