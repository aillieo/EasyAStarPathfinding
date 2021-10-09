using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class AStarTile : AStar<Tile>, ITileMapSolver
    {
        public AStarTile(ITileMapData tileData, Algorithms algorithm)
            : base(tileData, algorithm)
        {
        }
    }
}
