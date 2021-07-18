using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public abstract class BreadthFS : ISolver
    {
        public PathfindingState state => throw new NotImplementedException();

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
