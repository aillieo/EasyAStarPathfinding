using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{

    public class WayPointMapEditorCtrl
    {
        public float selectDistance = 2f;

        public int polygonHover = -1;

        public int polygonSelected = -1;
        public int lineSelected = -1;
        public int pointSelected = -1;

        public bool mouseOverLine;
        public bool mouseOverPoint;
        public bool selectingPoint;

        public int connectingStartPoint = -1;
        public Vector2 mousePosition2D;

        public Vector2 beginDragPosition;

        public void DeselectAll()
        {
            polygonHover = -1;

            polygonSelected = -1;
            pointSelected = -1;
            lineSelected = -1;

            mouseOverLine = false;
            mouseOverPoint = false;
            selectingPoint = false;

            connectingStartPoint = -1;
            mousePosition2D = default;
        }

        public void CleanUp()
        {
            DeselectAll();
        }
    }
}
