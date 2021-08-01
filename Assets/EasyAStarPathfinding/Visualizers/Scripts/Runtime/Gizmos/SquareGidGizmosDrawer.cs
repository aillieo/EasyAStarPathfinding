using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class SquareGidGizmosDrawer : IVisualizer<Grid>
    {
        public bool drawOpenList;
        public bool drawClosedList;

        public void Draw(Pathfinder pathfinder)
        {
            Visualize(pathfinder);
        }

        public void Visualize(Pathfinder pathfinder)
        {
            PathfindingContext<Grid> context = (pathfinder.solver as AStar<Grid>).context;
            Color backup = Gizmos.color;

            if (drawOpenList)
            {
                Gizmos.color = Color.black;
                foreach (var p in context.openList)
                {
                    Vector3 position = new Vector3(p.node.x, p.node.y, 0);
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(position, $"{p.g},{p.h}");
#endif
                    Gizmos.DrawWireCube(position, Vector3.one * 0.4f);
                }
            }

            if (drawClosedList)
            {
                Gizmos.color = Color.white;
                foreach (var p in context.closedSet)
                {
                    Gizmos.DrawWireCube(new Vector3(p.x, p.y, 0), Vector3.one * 0.4f);
                }
            }

            Gizmos.color = backup;
        }
    }
}
