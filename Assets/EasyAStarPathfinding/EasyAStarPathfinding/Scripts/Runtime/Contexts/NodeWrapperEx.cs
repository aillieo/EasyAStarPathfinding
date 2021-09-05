using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class NodeWrapperEx<T> : NodeWrapper<T> where T : IGraphNode
    {
        public float rhs;

        public NodeWrapperEx(T node, NodeWrapper<T> previous)
            : base(node, previous)
        {
        }
    }
}
