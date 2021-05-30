using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class PointNode : IComparable<PointNode>
    {
        public Point point;
        public PointNode previous;
        public float g;
        public float h;
        private PointNode()
        {
        }

        internal static PointNodePool Pool()
        {
            return new PointNodePool();
        }

        public int CompareTo(PointNode other)
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

            return other.point.CompareTo(point);
        }

        internal class PointNodePool
        {
            internal PointNodePool()
            {
            }

            private Stack<PointNode> nodePool = new Stack<PointNode>();

            internal PointNode GetPointNode(Point point = default, PointNode parent = default)
            {
                PointNode newNode;
                if (nodePool.Count > 0)
                {
                    newNode = nodePool.Pop();
                    newNode.point = point;
                    newNode.previous = parent;
                }
                else
                {
                    newNode = new PointNode { point = point, previous = parent };
                }
                return newNode;
            }

            public void Recycle(PointNode node)
            {
                node.g = 0f;
                node.h = 0f;
                node.previous = null;
                nodePool.Push(node);
            }
        }
    }
}
