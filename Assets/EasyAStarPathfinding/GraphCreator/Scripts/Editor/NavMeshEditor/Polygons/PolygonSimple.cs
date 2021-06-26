using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator
{
    public class PolygonSimple
    {
        public readonly List<Vector2> points = new List<Vector2>();

        public virtual bool Validate()
        {
            if (points.Count <= 3)
            {
                return false;
            }

            // todo 检查自相交
            return true;
        }
    }
}

