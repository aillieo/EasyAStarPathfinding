using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IGraphData { }

    public interface IGraphData<T> : IGraphData
    {
        IEnumerable<T> CollectNeighbor(T current);

        IEnumerable<T> GetAllNodes();

        float DefaultHeuristicFunc(T node1, T node2);

        bool LineOfSight(T node1, T node2);
    }
}
