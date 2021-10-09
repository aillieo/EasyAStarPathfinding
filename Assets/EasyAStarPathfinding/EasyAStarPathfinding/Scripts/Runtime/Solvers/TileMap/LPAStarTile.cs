using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class LPAStarTile : LPAStar<Tile>, ITileMapSolver
    {
        public LPAStarTile(ITileMapData tileData, Algorithms algorithm)
            : base(tileData, algorithm)
        {
        }
    }
}
