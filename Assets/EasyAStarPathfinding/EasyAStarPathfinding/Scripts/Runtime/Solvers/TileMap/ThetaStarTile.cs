using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class ThetaStarTile : ThetaStar<Tile>, ITileMapSolver
    {
        public ThetaStarTile(ITileMapData tileData, Algorithms algorithm)
            : base(tileData, algorithm)
        {
        }
    }
}
