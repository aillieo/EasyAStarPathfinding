using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface ISolver<T>
    {
        PathfindingState state { get; }

        void Init(T startingNode, T endingNode);

        PathfindingState Step();

        void CleanUp();

        IEnumerable<T> GetResult();
    }
}
