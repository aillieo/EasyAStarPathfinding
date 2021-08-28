using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public abstract class BreadthFS<T> : AStar<T> where T : IGraphNode
    {
        protected BreadthFS(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float GetG(NodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper.previous == null)
            {
                return 0f;
            }
            return nodeWrapper.previous.g + 0.1f;
        }

        protected override float GetH(NodeWrapper<T> nodeWrapper)
        {
            return 0f;
        }
    }
}
