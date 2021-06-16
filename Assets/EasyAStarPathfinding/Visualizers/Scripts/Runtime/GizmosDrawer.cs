using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class GizmosDrawer : IVisualizer
    {
        public bool drawOpenList;
        public bool drawClosedList;

        public void Draw(Pathfinder pathfinder)
        {
            DrawContext(pathfinder.context);
        }

        public void DrawGraph(IGraphData graphData)
        {
        }

        public void DrawContext(PathfindingContext context)
        {
            Color backup = Gizmos.color;

            if (drawOpenList)
            {
                Gizmos.color = Color.black;
                foreach (var p in context.openList)
                {
                    Vector3 position = new Vector3(p.point.x, p.point.y, 0);
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(position, $"{p.g},{p.h}");
#endif
                    Gizmos.DrawCube(position, Vector3.one * 0.4f);
                }
            }

            if (drawClosedList)
            {
                Gizmos.color = Color.white;
                foreach (var p in context.closedSet)
                {
                    Gizmos.DrawCube(new Vector3(p.x, p.y, 0), Vector3.one * 0.4f);
                }
            }

            Gizmos.color = backup;
        }
    }
}