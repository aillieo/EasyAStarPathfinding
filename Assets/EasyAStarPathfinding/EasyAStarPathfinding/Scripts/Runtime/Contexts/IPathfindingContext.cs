using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IPathfindingContext<T, out R> where R : INodeWrapper<T>
    {
        IGraphData<T> GetGraphData();

        R TryGetNode(T nodeData);

        R GetOrCreateNode(T nodeData);

        IEnumerable<R> GetAllNodes();

        void Reset();
    }
}
