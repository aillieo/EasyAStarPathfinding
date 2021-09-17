using System;

namespace AillieoUtils.Pathfinding
{
    public abstract class DepthFS<T> : AStar<T> where T : IGraphNode
    {
        protected DepthFS(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float CalculateG(NodeWrapper<T> nodeWrapper)
        {
            if (nodeWrapper.previous == null)
            {
                return float.PositiveInfinity;
            }
            return nodeWrapper.previous.g - 0.1f;
        }

        protected override float CalculateH(NodeWrapper<T> nodeWrapper)
        {
            return 0f;
        }
    }
}
