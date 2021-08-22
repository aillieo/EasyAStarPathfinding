using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IPathfindingContext<T> where T : IGraphNode
    {
        bool IsEndingNode(T node);

        NodePointer<T> TryGetOpenNode(T nodeData);

        NodePointer<T> TryGetClosedNode(T nodeData);

        IGraphData<T> GetGraphData();
    }
}
