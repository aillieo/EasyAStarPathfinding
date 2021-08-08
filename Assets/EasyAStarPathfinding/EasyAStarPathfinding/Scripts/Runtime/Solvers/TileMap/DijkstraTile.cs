using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class DijkstraTile : Dijkstra<Tile>, ITileMapSolver
    {
        public DijkstraTile(IGraphData<Tile> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }

        protected override float HeuristicFunc(Tile nodeFrom, Tile nodeTo)
        {
            return HeuristicFuncPreset.ManhattanDist((Vector2)nodeFrom, (Vector2)nodeTo);
        }
    }
}
