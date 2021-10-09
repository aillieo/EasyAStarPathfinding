using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IGraphData { }

    public interface IGraphData<T> : IGraphData where T : IGraphNode
    {
        IEnumerable<T> CollectNeighbor(T current);

        IEnumerable<T> GetAllNodes();

        float DefaultHeuristicFunc(T node1, T node2);
    }
}
