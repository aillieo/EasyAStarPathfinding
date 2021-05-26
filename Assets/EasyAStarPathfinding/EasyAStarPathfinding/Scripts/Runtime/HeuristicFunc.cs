using System;

namespace AillieoUtils
{
    public delegate float HeuristicFunc(Point point1, Point point2);

    public static class HeuristicFuncPreset
    {
        public static HeuristicFunc DefaultCostFunc { get; set; } = ManhattanDist;

        public static float ManhattanDist(Point a, Point b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        private static readonly float sqrt2m2 = (float)Math.Sqrt(2) - 2;
        public static float DiagonalDist(Point a, Point b)
        {
            float dx = Math.Abs(a.x - b.x);
            float dy = Math.Abs(a.y - b.y);
            return (float)(dx + dy + (sqrt2m2 * Math.Min(dx, dy)));
        }

        public static float EuclideanDist(Point a, Point b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

    }
}
