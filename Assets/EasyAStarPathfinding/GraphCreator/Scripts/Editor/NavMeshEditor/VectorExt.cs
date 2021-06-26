using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator
{
    public static class VectorExt
    {
        public static Vector3 ToV3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }
        public static Vector2 ToV2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
    }
}
