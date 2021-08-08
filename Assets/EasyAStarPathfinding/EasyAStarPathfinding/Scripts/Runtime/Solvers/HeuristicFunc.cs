using System;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public delegate float HeuristicFunc(Vector2 point1, Vector2 point2);

    public static class HeuristicFuncPreset
    {
        public static float ManhattanDist(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        private static readonly float sqrt2m2 = (float)Math.Sqrt(2) - 2;
        public static float DiagonalDist(Vector2 a, Vector2 b)
        {
            float dx = Math.Abs(a.x - b.x);
            float dy = Math.Abs(a.y - b.y);
            return (float)(dx + dy + (sqrt2m2 * Math.Min(dx, dy)));
        }

        public static float EuclideanDist(Vector2 a, Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
