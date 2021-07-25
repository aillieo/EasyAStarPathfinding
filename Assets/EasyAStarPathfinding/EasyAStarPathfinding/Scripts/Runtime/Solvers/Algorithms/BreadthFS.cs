using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public abstract class BreadthFS<T> : AStar<T> where T : IGraphNode
    {
        protected BreadthFS(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float GetG(NodePointer<T> nodePointer)
        {
            if (nodePointer.previous == null)
            {
                return 0f;
            }
            return nodePointer.previous.g + 0.1f;
        }

        protected override float GetH(NodePointer<T> nodePointer)
        {
            return 0f;
        }
    }
}
