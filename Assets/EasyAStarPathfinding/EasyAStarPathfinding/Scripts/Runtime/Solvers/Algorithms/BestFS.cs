using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public abstract class BestFS<T> : AStar<T> where T : IGraphNode
    {
        protected BestFS(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float GetG(NodePointer<T> nodePointer)
        {
            return 0f;
        }
    }
}
