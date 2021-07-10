using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Geometries
{
    public class Triangle2D
    {
        public Vector2 p0;
        public Vector2 p1;
        public Vector2 p2;

        public IEnumerable<Vector2> Points
        {
            get
            {
                yield return p0;
                yield return p1;
                yield return p2;
            }
        }

        public IEnumerable<Vector2> PointsReverse
        {
            get
            {
                yield return p2;
                yield return p1;
                yield return p0;
            }
        }

        public bool Clockwise()
        {
            // | 1    1    1  |
            // | x0   x1   x2 |
            // | y0   y1   y2 |
            float delta = p0.x * p1.y + p2.x * p0.y + p1.x * p2.y - p0.x * p2.y - p2.x * p1.y - p1.x * p0.y;
            return delta < 0;
        }
    }
}
