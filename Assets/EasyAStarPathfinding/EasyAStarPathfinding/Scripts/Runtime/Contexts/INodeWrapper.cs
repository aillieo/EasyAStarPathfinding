using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public interface INodeWrapper<out T> where T : IGraphNode
    {
        T node { get; }

        NodeState state { get; }
    }
}
