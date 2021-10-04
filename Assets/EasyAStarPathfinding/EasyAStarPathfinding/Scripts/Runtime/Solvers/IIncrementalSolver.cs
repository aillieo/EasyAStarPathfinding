using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IIncrementalSolver<T> : ISolver<T> where T : IGraphNode
    {
        void NotifyNodeDataModified(T nodeData);
    }
}
