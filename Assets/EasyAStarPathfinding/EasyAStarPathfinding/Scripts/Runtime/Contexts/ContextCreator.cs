using System;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    internal static class ContextCreator<T> where T : IGraphNode
    {
        internal static IPathfindingContext<T> CreateContext(IGraphData<T> graphData, Algorithms algorithm)
        {
            return new PathfindingContext<T>(graphData, algorithm);
        }
    }
}
