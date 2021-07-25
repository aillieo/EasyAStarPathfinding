using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.Pathfinding
{
    public class Pathfinder
    {
        internal readonly IGridMapSolver solver;

        public PathfindingState state => solver.state;
        public readonly Algorithms algorithm;

        public Pathfinder(IGraphData graphData, Algorithms algorithm = Algorithms.AStar)
        {
            this.algorithm = algorithm;
            this.solver = Solvers.Create(graphData, algorithm) as IGridMapSolver;
        }

        public void Init(Grid startPoint, Grid endPoint)
        {
            this.solver.Init(startPoint, endPoint);
        }

        public Grid[] FindPath(Grid startPoint, Grid endPoint)
        {
            Init(startPoint, endPoint);
            while (true)
            {
                if (solver.state == PathfindingState.Found || solver.state == PathfindingState.Failed)
                {
                    break;
                }

                FindPath();
            }

            return GetResult();
        }

        public Task<Grid[]> FindPathAsync(Grid startPoint, Grid endPoint)
        {
            TaskCompletionSource<Grid[]> tsc = new TaskCompletionSource<Grid[]>();
            ThreadPool.QueueUserWorkItem(_ => {
                var points = this.FindPath(startPoint, endPoint);
                tsc.SetResult(points);
            });
            return tsc.Task;
        }

        public PathfindingState FindPath()
        {
            switch (solver.state)
            {
                case PathfindingState.Uninitialized:
                    throw new Exception("Uninitialized PathfindingContext");
                case PathfindingState.Initialized:
                case PathfindingState.Finding:
                    solver.Step();
                    break;
                case PathfindingState.Found:
                case PathfindingState.Failed:
                    break;
            }

            return this.solver.state;
        }

        public virtual void CleanUp()
        {
            solver.CleanUp();
        }

        public void GetResult(List<Grid> toFill)
        {
            solver.GetResult(toFill);
        }

        public Grid[] GetResult()
        {
            List<Grid> result = new List<Grid>();
            GetResult(result);
            return result.ToArray();
        }
    }
}
