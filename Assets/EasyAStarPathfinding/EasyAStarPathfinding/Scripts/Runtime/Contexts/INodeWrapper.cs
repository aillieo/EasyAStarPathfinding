using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface INodeWrapper<T> where T : IGraphNode
    {
        T node { get; }

        NodeState state { get; }
    }
}
