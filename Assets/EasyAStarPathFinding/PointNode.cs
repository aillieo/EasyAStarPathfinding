using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    public class PointNode
    {
        public Point point;
        public PointNode previous;

        private PointNode()
        {
        }

        internal static PointNodePool Pool()
        {
            return new PointNodePool();
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
                node.previous = null;
                nodePool.Push(node);
            }
        }
    }
}
