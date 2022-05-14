using System;

namespace AillieoUtils.Pathfinding
{
    public class Dijkstra<T> : AStar<T>
    {
        public Dijkstra(IGraphData<T> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float CalculateH(NodeWrapper<T> nodeWrapper)
        {
            return 0f;
        }
    }
}
