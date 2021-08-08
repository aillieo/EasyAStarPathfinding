using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.Pathfinding
{
    public class Pathfinder
    {
        internal readonly ITileMapSolver solver;

        public PathfindingState state => solver.state;
        public readonly Algorithms algorithm;

        public Pathfinder(IGraphData graphData, Algorithms algorithm = Algorithms.AStar)
        {
            this.algorithm = algorithm;
            this.solver = Solvers.Create(graphData, algorithm) as ITileMapSolver;
        }

        public void Init(Tile startPoint, Tile endPoint)
        {
            this.solver.Init(startPoint, endPoint);
        }

        public Tile[] FindPath(Tile startPoint, Tile endPoint)
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

        public Task<Tile[]> FindPathAsync(Tile startPoint, Tile endPoint)
        {
            TaskCompletionSource<Tile[]> tsc = new TaskCompletionSource<Tile[]>();
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

        public void GetResult(List<Tile> toFill)
        {
            solver.GetResult(toFill);
        }

        public Tile[] GetResult()
        {
            List<Tile> result = new List<Tile>();
            GetResult(result);
            return result.ToArray();
        }
    }
}
