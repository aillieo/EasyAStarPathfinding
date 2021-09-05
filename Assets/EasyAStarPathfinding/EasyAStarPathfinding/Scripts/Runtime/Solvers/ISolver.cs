using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface ISolver<out T> where T : IGraphNode
    {
        PathfindingState state { get; }

        void Init();

        PathfindingState Step();

        void CleanUp();
    }
}
