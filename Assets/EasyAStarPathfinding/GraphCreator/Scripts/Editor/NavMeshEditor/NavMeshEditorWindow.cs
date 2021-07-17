using AillieoUtils.Geometries;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{
    public class NavMeshEditorWindow : BaseEditorWindow<NavMeshData>
    {
        [MenuItem("AillieoUtils/AStarPathfinding/NavMeshEditorWindow")]
        public static void Open()
        {
            GetWindowAndOpen<NavMeshEditorWindow>();
        }

        public class PolygonWithExtraInfo
        {
            public readonly PolygonSimple polygon = new PolygonSimple();
            public Color color = Color.white;
            public string name = string.Empty;
        }

        private static readonly Color colorSelected = Color.red;
        private static readonly Color colorInactive = Color.gray;
        private static readonly Color colorActive = Color.white;

        private List<PolygonWithExtraInfo> simplePolygons = new List<PolygonWithExtraInfo>();
        private Mesh mesh;
        private Mesh savedMesh;

        private NavMeshEditorCtrl ctrl;

        private bool sceneDirty = false;
        private bool guiDirty = false;

        public void OnGUI()
        {
            DrawSaveLoadButtons();

            savedMesh = EditorGUILayout.ObjectField(savedMesh, typeof(Mesh), false) as Mesh;
            GUILayout.Button("Save to mesh");
            GUILayout.Button("Load from mesh");



            EditorGUILayout.Space(10);
            ctrl.selectDistance = EditorGUILayout.Slider(new GUIContent("Vert size"), ctrl.selectDistance, 0.1f, 8f);

            if (GUILayout.Button("Create new polygon"))
            {
                CreateNewPolygon();
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

            //if (mesh == null)
            //{
            //    mesh = new Mesh();
            //}
        }

        protected override void SceneCleanUp()
        {
            //if (mesh != null)
            //{
            //    DestroyImmediate(mesh);
            //    mesh = null;
            //}
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

        private void CreateNewPolygon()
        {
            var newPolygon = new PolygonWithExtraInfo();
            simplePolygons.Add(newPolygon);
            int index = simplePolygons.Count - 1;
            newPolygon.name = $"Polygon {(simplePolygons.Count - 1):d2}";
            ctrl.polygonSelected = index;
        }

        private void TryAddNewPoint(Vector2 position)
        {
            var selectedPoly = simplePolygons[ctrl.polygonSelected].polygon;
            int newPointIndex = selectedPoly.verts.Count;
            FindClosestLineForPolygon(position, ctrl.polygonSelected, out int lineIndex);
            if (lineIndex != -1)
            {
                newPointIndex = lineIndex + 1;
            }

            selectedPoly.verts.Insert(newPointIndex, position);
            ctrl.pointHover = newPointIndex;
            ctrl.pointSelected = newPointIndex;
            ctrl.polygonHover = ctrl.polygonSelected;

            sceneDirty = true;
            guiDirty = true;

            TrySelectPoint();
        }

        private void TrySelectPoint()
        {
            if (ctrl.pointHover < 0)
            {
                return;
            }

            ctrl.selectingPoint = true;
            ctrl.lineHover = -1;
            ctrl.lineSelected = -1;

            var polygonSelected = simplePolygons[ctrl.polygonSelected].polygon;
            ctrl.beginDragPosition = polygonSelected.verts[ctrl.pointSelected];

            sceneDirty = true;
        }

        private void TrySelectPolygon()
        {
            if (ctrl.polygonHover != -1)
            {
                ctrl.polygonSelected = ctrl.polygonHover;
                sceneDirty = true;
                guiDirty = true;
            }
        }

        private void OnMouseEvent(Event evt)
        {
            Vector2 mousePosition2D = GetMouseWorldPosition2D(evt);

            if (evt.button == 0)
            {
                if (evt.type == EventType.MouseDown && evt.modifiers == EventModifiers.Shift)
                {
                    if (ctrl.pointHover >= 0)
                    {
                        TrySelectPolygon();
                        var polygonSelected = simplePolygons[ctrl.polygonSelected].polygon;
                        polygonSelected.verts.RemoveAt(ctrl.pointSelected);
                        ctrl.selectingPoint = false;
                        ctrl.pointHover = -1;

                        sceneDirty = true;
                        guiDirty = true;
                    }
                    else
                    {
                        TryAddNewPoint(mousePosition2D);
                    }
                }

                if (evt.type == EventType.MouseDown && evt.modifiers == EventModifiers.None)
                {
                    if (evt.clickCount == 2)
                    {
                        TrySelectPolygon();
                    }

                    if (ctrl.pointHover >= 0)
                    {
                        TrySelectPolygon();
                        TrySelectPoint();
                    }
                }

                if (evt.type == EventType.MouseUp)
                {
                    if (ctrl.selectingPoint)
                    {
                        var polygonSelected = simplePolygons[ctrl.polygonSelected].polygon;
                        polygonSelected.verts[ctrl.pointSelected] = ctrl.beginDragPosition;
                        polygonSelected.verts[ctrl.pointSelected] = mousePosition2D;

                        ctrl.selectingPoint = false;
                        ctrl.pointSelected = -1;
                        sceneDirty = true;
                    }
                }

                if (evt.type == EventType.MouseDrag)
                {
                    if (ctrl.selectingPoint)
                    {
                        var polygonSelected = simplePolygons[ctrl.polygonSelected].polygon;
                        polygonSelected.verts[ctrl.pointSelected] = mousePosition2D;
                        sceneDirty = true;
                    }
                }
            }

            if (!ctrl.selectingPoint)
            {
                UpdateMouseOverStates(mousePosition2D);
            }
        }

        private float FindClosestLineForPolygon(Vector2 mousePosition2D, int polygon, out int lineIndex)
        {
            lineIndex = -1;
            float dist = float.MaxValue;
            var poly = simplePolygons[polygon].polygon;

            for (int i = 0; i < poly.verts.Count; i++)
            {
                float newDist = HandleUtility.DistancePointToLineSegment(
                    mousePosition2D,
                    poly.verts[i],
                    poly.verts[(i + 1) % poly.verts.Count]);

                if (newDist < dist)
                {
                    dist = newDist;
                    lineIndex = i;
                }
            }

            return dist;
        }

        private float FindClosestLine(Vector2 mousePosition2D, out int polygon, out int lineIndex)
        {
            polygon = -1;
            lineIndex = -1;
            float dist = float.MaxValue;
            for (int i = 0; i < simplePolygons.Count; i++)
            {
                float newDist = FindClosestLineForPolygon(mousePosition2D, i, out int newLineIndex);
                if (newDist < dist)
                {
                    dist = newDist;
                    polygon = i;
                    lineIndex = newLineIndex;
                }
            }

            return dist;
        }

        private bool FindMouseOverPoint(Vector2 mousePosition2D, out int polygon, out int pointIndex)
        {
            pointIndex = -1;
            polygon = -1;
            for (int i = 0; i < simplePolygons.Count; i++)
            {
                var p = simplePolygons[i].polygon;
                for (int j = 0; j < p.verts.Count; j++)
                {
                    if (Vector2.Distance(mousePosition2D, p.verts[j]) < ctrl.selectDistance)
                    {
                        pointIndex = j;
                        polygon = i;
                        return true;
                    }
                }
            }
            return false;
        }

        private void UpdateMouseOverStates(Vector2 mousePosition2D)
        {
            if (!FindMouseOverPoint(mousePosition2D, out int mouseOverPolygon, out int mouseOverPoint))
            {
                mouseOverPolygon = -1;
                mouseOverPoint = -1;
            }

            if (mouseOverPoint != ctrl.pointSelected || mouseOverPolygon != ctrl.polygonHover)
            {
                ctrl.polygonHover = mouseOverPolygon;
                ctrl.pointSelected = mouseOverPoint;
                ctrl.pointHover = mouseOverPoint;

                sceneDirty = true;
            }

            if (ctrl.pointHover >= 0)
            {
                ctrl.lineHover = -1;
                ctrl.lineSelected = -1;
            }
            else
            {
                if (FindClosestLine(mousePosition2D, out mouseOverPolygon, out int mouseOverLineIndex) > ctrl.selectDistance)
                {
                    mouseOverPolygon = -1;
                    mouseOverLineIndex = -1;
                }

                if (ctrl.lineSelected != mouseOverLineIndex || mouseOverPolygon != ctrl.polygonHover)
                {
                    ctrl.polygonHover = mouseOverPolygon;
                    ctrl.lineSelected = mouseOverLineIndex;
                    ctrl.lineHover = mouseOverLineIndex;
                    sceneDirty = true;
                }
            }
        }

        private void DrawPolygonsInScene()
        {
            Color backup = Handles.color;
            for (int i = 0; i < simplePolygons.Count; i++)
            {
                PolygonWithExtraInfo polyEx = simplePolygons[i];
                PolygonSimple poly = polyEx.polygon;
                bool polygonSelected = i == ctrl.polygonSelected;
                bool polygonHover = i == ctrl.polygonHover;

                for (int j = 0; j < poly.verts.Count; j++)
                {
                    Vector3 thisPoint = poly.verts[j].ToV3();
                    Vector3 nextPoint = poly.verts[(j + 1) % poly.verts.Count].ToV3();
                    if (j == ctrl.lineSelected && polygonHover)
                    {
                        Handles.color = colorSelected;
                        Handles.DrawLine(thisPoint, nextPoint);
                    }
                    else
                    {
                        Handles.color = polygonSelected ? colorActive : colorInactive;
                        Handles.DrawDottedLine(thisPoint, nextPoint, 4f);
                    }

                    if (j == ctrl.pointSelected && polygonHover)
                    {
                        Handles.color = colorSelected;
                    }
                    else
                    {
                        Handles.color = polygonSelected ? colorActive : colorInactive;
                    }
                    Handles.DrawSolidDisc(thisPoint, Vector3.up, ctrl.selectDistance);
                }
            }

            Handles.color = backup;
            sceneDirty = false;
        }

        private void DrawPolygonsGUI()
        {
            // 检查是否有删除 begin
            int polyToDelete = -1;

            for (int i = 0; i < simplePolygons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                simplePolygons[i].name = EditorGUILayout.TextField(simplePolygons[i].name);

                GUILayout.Label($"{simplePolygons[i].polygon.verts.Count}");

                bool polygonSelected = i == ctrl.polygonSelected;
                GUI.enabled = !polygonSelected;
                if (GUILayout.Button("Activate"))
                {
                    ctrl.polygonSelected = i;
                }
                GUI.enabled = true;

                if (GUILayout.Button("Validate"))
                {
                    bool valid = simplePolygons[i].polygon.Validate();
                    if (valid)
                    {
                        // todo
                    }
                }

                simplePolygons[i].color = EditorGUILayout.ColorField(simplePolygons[i].color);

                if (GUILayout.Button("[ x ]"))
                {
                    polyToDelete = i;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (polyToDelete != -1)
            {
                simplePolygons.RemoveAt(polyToDelete);
                if (ctrl.polygonSelected >= polyToDelete)
                {
                    ctrl.polygonSelected -= 1;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ctrl = new NavMeshEditorCtrl();
            CreateNewPolygon();

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
