using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class HPA<T> : ISolver<T>
    {
        public HPA(IGraphData<T> graphData, Algorithms algorithm)
        {
        }

        public PathfindingState state => throw new NotImplementedException();

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetResult()
        {
            throw new NotImplementedException();
        }

        public void Init(T startingNode, T endingNode)
        {
            throw new NotImplementedException();
        }

        public PathfindingState Step()
        {
            throw new NotImplementedException();
        }
    }
}
