using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AillieoUtils.Pathfinding
{
    public class Pathfinder
    {
        internal readonly ISolver<Tile> solver;

        public PathfindingState state => solver.state;
        public readonly Algorithms algorithm;

        public Pathfinder(IGraphData<Tile> graphData, Algorithms algorithm = Algorithms.AStar)
        {
            this.algorithm = algorithm;
            this.solver = SolverCreator.Create(graphData, algorithm);
        }

        public void Init(Tile startTile, Tile endingTile)
        {
            this.solver.Init(startTile, endingTile);
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
            toFill.Clear();
            toFill.AddRange(solver.GetResult());
        }

        public Tile[] GetResult()
        {
            return solver.GetResult().ToArray();
        }

        public void NotifyNodeDataModified<T>(T nodeData)
        {
            if (solver is IIncrementalSolver<T> incrementalSolver)
            {
                incrementalSolver.NotifyNodeDataModified(nodeData);
            }
        }
    }
}
