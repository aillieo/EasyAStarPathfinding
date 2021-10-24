using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class DijkstraTile : Dijkstra<Tile>
    {
        public DijkstraTile(IGraphData<Tile> graphData, Algorithms algorithm) : base(graphData, algorithm)
        {
        }
    }
}
