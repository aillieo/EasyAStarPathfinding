using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class NodePointer<T> : IComparable<NodePointer<T>> where T : IGraphNode
    {
        public T node;
        public NodePointer<T> previous;
        public float g;
        public float h;
        private NodePointer()
        {
        }

        internal static NodePointerPool Pool()
        {
            return new NodePointerPool();
        }

        public int CompareTo(NodePointer<T> other)
        {
            float f1 = h + g;
            float f2 = other.h + other.g;

            if (f1 != f2)
            {
                return f2.CompareTo(f1);
            }

            if (h != other.h)
            {
                return other.h.CompareTo(h);
            }

            return other.CompareTo(this);
        }

        internal class NodePointerPool
        {
            internal NodePointerPool()
            {
            }

            private Stack<NodePointer<T>> nodePool = new Stack<NodePointer<T>>();

            internal NodePointer<T> GetPointNode(T grid = default, NodePointer<T> parent = default)
            {
                NodePointer<T> newPointer;
                if (nodePool.Count > 0)
                {
                    newPointer = nodePool.Pop();
                    newPointer.node = grid;
                    newPointer.previous = parent;
                }
                else
                {
                    newPointer = new NodePointer<T> { node = grid, previous = parent };
                }
                return newPointer;
            }

            public void Recycle(NodePointer<T> pointer)
            {
                pointer.g = 0f;
                pointer.h = 0f;
                pointer.previous = null;
                nodePool.Push(pointer);
            }
        }
    }
}
