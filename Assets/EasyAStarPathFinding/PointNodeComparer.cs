using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.PathFinding
{
    internal class PointNodeComparer : IComparer<PointNode>
    {
        private HeuristicFunc costFunc;
        private Point startPoint;
        private Point endPoint;

        public PointNodeComparer(HeuristicFunc costFunc)
        {
            this.costFunc = costFunc;
        }

        public void Init(Point startPoint, Point endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public int Compare(PointNode lhs, PointNode rhs)
        {
            float h1 = costFunc(lhs.point, endPoint);
            float h2 = costFunc(rhs.point, endPoint);
            float f1 = h1 + lhs.g;
            float f2 = h2 + rhs.g;

            if (f1 != f2)
            {
                return f2.CompareTo(f1);
            }
            return lhs.point.CompareTo(rhs.point);
        }
    }

}
