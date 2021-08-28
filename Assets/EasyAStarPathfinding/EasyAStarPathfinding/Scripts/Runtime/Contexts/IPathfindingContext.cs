using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IPathfindingContext<T> where T : IGraphNode
    {
        NodeWrapper<T> TryGetOpenNode(T nodeData);

        NodeWrapper<T> TryGetClosedNode(T nodeData);

        IGraphData<T> GetGraphData();

        bool RemoveFromMapping(T nodeData);

        void AddToOpen(T nodeData, NodeWrapper<T> nodeWrapper);

        void AddToClosed(T nodeData, NodeWrapper<T> nodeWrapper);

        IEnumerable<NodeWrapper<T>> GetAllNodes();

        NodeWrapper<T> TryGetFrontier();

        void UpdateFrontier(NodeWrapper<T> nodeWrapper);

        NodeWrapper<T> CreateNewNode(T node, NodeWrapper<T> previous);

        void Reset();
    }
}
