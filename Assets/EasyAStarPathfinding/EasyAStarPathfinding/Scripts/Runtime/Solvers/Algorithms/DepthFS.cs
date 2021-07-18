using System;

namespace AillieoUtils.Pathfinding
{
    public abstract class DepthFS : ISolver
    {
        public PathfindingState state => throw new NotImplementedException();

        public DepthFS(IGridMapData gridData, Algorithms algorithm)
        {
        }

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public PathfindingState Step()
        {
            throw new NotImplementedException();
        }
    }
}
