using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.PathFinding
{
    internal class PointNodeComparer : IComparer<PointNode>
    {
        private CostFunc costFunc;
        private Point startPoint;
        private Point endPoint;

        public PointNodeComparer(CostFunc costFunc)
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
            int cost1 = costFunc(lhs.point, startPoint, endPoint);
            int cost2 = costFunc(rhs.point, startPoint, endPoint);
            if (cost1 != cost2)
            {
                return cost1 - cost2;
            }
            return lhs.point.CompareTo(rhs.point);
        }
    }

}
