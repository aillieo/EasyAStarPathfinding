using System;

namespace AillieoUtils.Pathfinding
{
    public abstract class Dijkstra<T> : AStar<T> where T : IGraphNode
    {
        protected Dijkstra(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float GetH(NodePointer<T> nodePointer)
        {
            return 0f;
        }
    }
}
