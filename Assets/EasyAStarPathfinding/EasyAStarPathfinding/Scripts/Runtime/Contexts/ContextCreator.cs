using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    internal static class ContextCreator<T> where T : IGraphNode
    {
        internal static IPathfindingContext<T, INodeWrapper<T>> CreateContext(IGraphData<T> graphData, Algorithms algorithm)
        {
            switch (algorithm)
            {
                case Algorithms.LPAStar:
                case Algorithms.DStarLite:
                    return new PathfindingContextEx<T>(graphData, algorithm);
                default:
                    return new PathfindingContext<T>(graphData, algorithm);
            }
        }
    }
}
