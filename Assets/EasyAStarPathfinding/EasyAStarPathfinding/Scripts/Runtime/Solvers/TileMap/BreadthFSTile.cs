using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class BreadthFSTile : BreadthFS<Tile>, ITileMapSolver
    {
        public BreadthFSTile(IGraphData<Tile> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float HeuristicFunc(Tile nodeFrom, Tile nodeTo)
        {
            return HeuristicFuncPreset.ManhattanDist((Vector2)nodeFrom, (Vector2)nodeTo);
        }
    }
}
