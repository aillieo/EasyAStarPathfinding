using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{

    public class WayPointMapEditorWindow : BaseEditorWindow<WayPointData>
    {
        [MenuItem("AillieoUtils/AStarPathfinding/WayPointMapEditorWindow")]
        public static void Open()
        {
            GetWindowAndOpen<WayPointMapEditorWindow>();
        }

        public class PointInfo
        {
            public string name;
            public Vector2 point;
        }

        public class LineInfo
        {
            public PointInfo point0;
            public PointInfo point1;
        }

        private static readonly Color colorSelected = Color.red;
        private static readonly Color colorInactive = Color.gray;
        private static readonly Color colorActive = Color.white;

        private List<PointInfo> points = new List<PointInfo>();
        private List<LineInfo> lines = new List<LineInfo>();

        private WayPointMapEditorCtrl ctrl;

        private bool sceneDirty = false;
        private bool guiDirty = false;

        public void OnGUI()
        {
            DrawSaveLoadButtons();

            EditorGUILayout.Space(10);
            ctrl.selectDistance = EditorGUILayout.Slider(new GUIContent("Vert size"), ctrl.selectDistance, 0.1f, 8f);

            if (GUILayout.Button("Create new polygon"))
            {
            }

            DrawPolygonsGUI();

            if (GUI.changed)
            {
                sceneDirty = true;
                SceneView.RepaintAll();
            }
        }

        protected override void SceneInit()
        {
            var sceneView = SceneView.lastActiveSceneView;
            Vector3 center = Vector3.zero;
            sceneView.in2DMode = false;
            sceneView.LookAt(center, new Quaternion(1, 0, 0, 1), 200, true, false);
            sceneView.Repaint();
        }

        protected override void SceneCleanUp()
        {
        }

        protected override void OnSceneGUI(SceneView sceneView)
        {
            Event current = Event.current;

            if (current.type == EventType.Repaint)
            {
                DrawPolygonsInScene();
            }
            else if (current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            else if (current.isMouse)
            {
                // 比较复杂了 各种鼠标操作
                OnMouseEvent(current);

                if (sceneDirty)
                {
                    HandleUtility.Repaint();
                    sceneDirty = false;
                }

                if (guiDirty)
                {
                    this.Repaint();
                    guiDirty = false;
                }
            }
        }

        private void TryAddNewPoint(Vector2 position)
        {
            PointInfo pointInfo = new PointInfo() { point = position };
            pointInfo.name = $"Point {(points.Count - 1):d2}";
            points.Add(pointInfo);

            sceneDirty = true;
            guiDirty = true;

            TrySelectPoint();
        }

        private void TrySelectPoint()
        {
            if (!ctrl.mouseOverPoint)
            {
                return;
            }

            ctrl.selectingPoint = true;
            ctrl.mouseOverLine = false;
            ctrl.lineSelected = -1;

            sceneDirty = true;
        }

        private bool TryAddLine(int p0, int p1)
        {
            if (p0 == p1)
            {
                return false;
            }

            if (p0 > p1)
            {
                var p = p0;
                p0 = p1;
                p1 = p;
            }

            PointInfo pi0 = points[p0];
            PointInfo pi1 = points[p1];

            foreach (var l in lines)
            {
                if (l.point0 == pi0 && l.point1 == pi1)
                {
                    return false;
                }
            }

            LineInfo line = new LineInfo
            {
                point0 = pi0,
                point1 = pi1,
            };
            lines.Add(line);

            return true;
        }

        private void RemovePoint(int point)
        {
            PointInfo pointToRemove = points[point];
            for (int i = lines.Count - 1; i >= 0; --i)
            {
                LineInfo testLine = lines[i];
                if (testLine.point0 == pointToRemove || testLine.point1 == pointToRemove)
                {
                    lines.RemoveAt(i);
                }
            }
            points.RemoveAt(point);
        }

        private void RemoveLine(int line)
        {
            lines.RemoveAt(line);
        }

        private void OnMouseEvent(Event evt)
        {
            Vector2 mousePosition2D = GetMouseWorldPosition2D(evt);

            if (evt.type == EventType.MouseMove && ctrl.connectingStartPoint > -1)
            {
                sceneDirty = true;
            }

            if (evt.button == 0)
            {
                if (evt.type == EventType.MouseDown && evt.modifiers == EventModifiers.Shift)
                {
                    ctrl.connectingStartPoint = -1;

                    if (ctrl.mouseOverPoint)
                    {
                        RemovePoint(ctrl.pointSelected);
                        ctrl.selectingPoint = false;
                        ctrl.mouseOverPoint = false;

                        sceneDirty = true;
                        guiDirty = true;
                    }

                    if (ctrl.mouseOverLine)
                    {
                        RemoveLine(ctrl.lineSelected);
                        ctrl.mouseOverLine = false;
                        ctrl.lineSelected = -1;

                        sceneDirty = true;
                        guiDirty = true;
                    }
                }

                if (evt.type == EventType.MouseDown && evt.modifiers == EventModifiers.None)
                {
                    if (ctrl.mouseOverPoint)
                    {
                        TrySelectPoint();
                    }

                    if (evt.clickCount == 2)
                    {
                        if (ctrl.mouseOverPoint)
                        {
                            ctrl.connectingStartPoint = ctrl.pointSelected;
                        }
                        else
                        {
                            TryAddNewPoint(mousePosition2D);
                            ctrl.connectingStartPoint = -1;
                        }
                    }
                    else
                    {
                        if (ctrl.mouseOverPoint)
                        {
                            if (ctrl.connectingStartPoint >= 0)
                            {
                                TryAddLine(ctrl.connectingStartPoint, ctrl.pointSelected);
                                ctrl.connectingStartPoint = -1;
                            }
                        }
                        else
                        {
                            ctrl.connectingStartPoint = -1;
                        }
                    }

                    guiDirty = true;
                    sceneDirty = true;
                }

                if (evt.type == EventType.MouseUp)
                {
                    if (ctrl.selectingPoint)
                    {
                        ctrl.selectingPoint = false;
                        ctrl.pointSelected = -1;
                        sceneDirty = true;
                    }
                }

                if (evt.type == EventType.MouseDrag)
                {
                    if (ctrl.selectingPoint)
                    {
                        PointInfo pointInfo = points[ctrl.pointSelected];
                        pointInfo.point = mousePosition2D;
                        sceneDirty = true;
                    }
                }
            }

            if (!ctrl.selectingPoint)
            {
                UpdateMouseOverStates(mousePosition2D);
            }
        }

        private float FindClosestLine(Vector2 mousePosition2D, out int lineIndex)
        {
            lineIndex = -1;
            float dist = float.MaxValue;

            for (int i = 0; i < lines.Count; i++)
            {
                LineInfo line = lines[i];
                float newDist = HandleUtility.DistancePointToLineSegment(
                    mousePosition2D,
                    line.point0.point,
                    line.point1.point);

                if (newDist < dist)
                {
                    dist = newDist;
                    lineIndex = i;
                }
            }

            return dist;
        }

        private bool FindMouseOverPoint(Vector2 mousePosition2D, out int pointIndex)
        {
            pointIndex = -1;
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (Vector2.Distance(mousePosition2D, p.point) < ctrl.selectDistance)
                {
                    pointIndex = i;
                    return true;
                }
            }
            return false;
        }

        private void UpdateMouseOverStates(Vector2 mousePosition2D)
        {
            ctrl.mousePosition2D = mousePosition2D;

            int mouseOverPolygon = -1;
            if (!FindMouseOverPoint(mousePosition2D, out int mouseOverPoint))
            {
                mouseOverPolygon = -1;
                mouseOverPoint = -1;
            }

            if (mouseOverPoint != ctrl.pointSelected || mouseOverPolygon != ctrl.polygonHover)
            {
                ctrl.polygonHover = mouseOverPolygon;
                ctrl.pointSelected = mouseOverPoint;
                ctrl.mouseOverPoint = mouseOverPoint != -1;

                sceneDirty = true;
            }

            if (ctrl.mouseOverPoint)
            {
                ctrl.mouseOverLine = false;
                ctrl.lineSelected = -1;
            }
            else
            {
                if (FindClosestLine(mousePosition2D, out int mouseOverLineIndex) > ctrl.selectDistance)
                {
                    mouseOverPolygon = -1;
                    mouseOverLineIndex = -1;
                }

                if (ctrl.lineSelected != mouseOverLineIndex || mouseOverPolygon != ctrl.polygonHover)
                {
                    ctrl.polygonHover = mouseOverPolygon;
                    ctrl.lineSelected = mouseOverLineIndex;
                    ctrl.mouseOverLine = mouseOverLineIndex != -1;
                    sceneDirty = true;
                }
            }
        }

        private void DrawPolygonsInScene()
        {
            Color backup = Handles.color;

            for (int i = 0; i < points.Count; i++)
            {
                PointInfo pointInfo = points[i];

                if (i == ctrl.pointSelected)
                {
                    Handles.color = colorSelected;
                }
                else
                {
                    Handles.color = colorActive;
                }

                Vector3 worldPosition = new Vector3(pointInfo.point.x, 0, pointInfo.point.y);
                Handles.DrawSolidDisc(worldPosition, Vector3.up, ctrl.selectDistance);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                LineInfo line = lines[i];
                bool selected = ctrl.connectingStartPoint < 0 && (i == ctrl.lineSelected);
                Handles.color = selected ? colorSelected : colorActive;
                Handles.DrawLine(
                    line.point0.point.ToV3(),
                    line.point1.point.ToV3());
            }

            Handles.color = colorInactive;
            if (ctrl.connectingStartPoint >= 0)
            {
                Vector3 lineStart = points[ctrl.connectingStartPoint].point.ToV3();
                Vector3 mouse = ctrl.mousePosition2D.ToV3();

                Handles.DrawLine(lineStart, mouse);
            }

            Handles.color = backup;
            sceneDirty = false;
        }

        private void DrawPolygonsGUI()
        {
            GUILayout.Label($"Point:{points.Count} Line:{lines.Count}");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ctrl = new WayPointMapEditorCtrl();

            Tools.hidden = true;
        }

        protected override void OnDisable()
        {
            if (ctrl != null)
            {
                ctrl.CleanUp();
                ctrl = null;
            }

            Tools.hidden = false;

            base.OnDisable();
        }
    }
}
