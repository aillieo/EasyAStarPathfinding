using System;

namespace AillieoUtils
{
    public delegate int CostFunc(Point point, Point startPoint, Point endPoint);

    public static class CostFuncPreset
    {
        public static int DefaultCostFunc(Point current, Point startPoint, Point endPoint)
        {
            return Math.Abs(startPoint.x - current.x) +
                Math.Abs(startPoint.y - current.y) +
                Math.Abs(endPoint.x - current.x) +
                Math.Abs(endPoint.y - current.y);
        }
    }
}
