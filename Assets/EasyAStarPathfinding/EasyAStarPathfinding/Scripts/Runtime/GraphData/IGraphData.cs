using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IGraphData { }

    public interface IGraphData<T> : IGraphData
    {
        IEnumerable<T> CollectNeighbor(T current);
    }
}
