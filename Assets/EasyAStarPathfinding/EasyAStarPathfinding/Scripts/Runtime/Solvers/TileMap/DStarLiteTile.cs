using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class DStarLiteTile : DStarLite<Tile>
    {
        public DStarLiteTile(ITileMapData tileData, Algorithms algorithm)
            : base(tileData, algorithm)
        {
        }
    }
}
