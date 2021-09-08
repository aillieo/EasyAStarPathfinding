using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IPathfindingContext<T, out R> where T : IGraphNode where R : INodeWrapper<T>
    {
        R TryGetOpenNode(T nodeData);

        R TryGetClosedNode(T nodeData);

        IGraphData<T> GetGraphData();

        bool RemoveFromMapping(T nodeData);

        void AddToOpen(T nodeData, INodeWrapper<T> nodeWrapper);

        void AddToClosed(T nodeData, INodeWrapper<T> nodeWrapper);

        IEnumerable<R> GetAllNodes();

        R TryGetFrontier();

        void UpdateFrontier(INodeWrapper<T> nodeWrapper);

        R GetOrCreateNode(T node, INodeWrapper<T> previous);

        void Reset();
    }
}
