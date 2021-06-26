using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator
{
    public class NavMeshEditorCtrl
    {
        public float selectDistance = 2f;

        public int polygonHover = -1;

        public int polygonSelected = -1;
        public int lineSelected = -1;
        public int pointSelected = -1;

        public bool mouseOverLine;
        public bool mouseOverPoint;
        public bool selectingPoint;

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
        }

        public void CleanUp()
        {
            DeselectAll();
        }
    }
}
