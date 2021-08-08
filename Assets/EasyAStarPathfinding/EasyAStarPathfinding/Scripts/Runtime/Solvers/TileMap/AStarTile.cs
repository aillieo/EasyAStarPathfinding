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

        protected override float HeuristicFunc(Tile nodeFrom, Tile nodeTo)
        {
            return HeuristicFuncPreset.ManhattanDist((Vector2)nodeFrom, (Vector2)nodeTo);
        }
    }
}
