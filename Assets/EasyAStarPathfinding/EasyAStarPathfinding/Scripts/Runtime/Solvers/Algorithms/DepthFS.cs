using System;

namespace AillieoUtils.Pathfinding
{
    public abstract class DepthFS<T> : AStar<T> where T : IGraphNode
    {
        protected DepthFS(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float GetG(NodePointer<T> nodePointer)
        {
            if (nodePointer.previous == null)
            {
                return float.MaxValue;
            }
            return nodePointer.previous.g - 0.1f;
        }

        protected override float GetH(NodePointer<T> nodePointer)
        {
            return 0f;
        }
    }
}
