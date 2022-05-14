using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface IIncrementalSolver<T> : ISolver<T>
    {
        void NotifyNodeDataModified(T nodeData);
    }
}
