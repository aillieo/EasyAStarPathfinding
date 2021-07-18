using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{

    public class SquareGridEditorWindow : BaseEditorWindow<SquareGridMapData>
    {
        [MenuItem("AillieoUtils/AStarPathfinding/SquareGridEditorWindow")]
        public static void Open()
        {
            GetWindowAndOpen<SquareGridEditorWindow>();
        }

        private GameObject goRoot;
        private Texture2D texture;
        private Texture2D savedTexture;

        private Vector2Int size;
        private float brushValue;
        private int brushSize;

        public void OnGUI()
        {
            DrawSaveLoadButtons();

            savedTexture = EditorGUILayout.ObjectField(savedTexture, typeof(Texture2D), false) as Texture2D;
            if (GUILayout.Button("Save to texture"))
            {
            }

            if (GUILayout.Button("Load from texture"))
            {
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size:");
            size = EditorGUILayout.Vector2IntField(GUIContent.none, size);
            if (GUILayout.Button("Resize"))
            {
                data.Resize(size.x, size.y);
            }
            EditorGUILayout.EndHorizontal();

            brushValue = EditorGUILayout.Slider("Brush", brushValue, 0, 1);
            brushSize = EditorGUILayout.IntSlider("BrushSize", brushSize, 1, 50);
        }

        protected override void Save()
        {
            for (int i = 0; i < data.RangeY; ++i)
            {
                for (int j = 0; j < data.RangeX; ++j)
                {
                    Color c = texture.GetPixel(i, j);
                    data.SetPassable(i, j, c.r > 0.5f);
                }
            }
            base.Save();
        }

        protected override void Load()
        {
            base.Load();
            CreateSceneRootForData();
            size = new Vector2Int(data.RangeX, data.RangeY);
            brushValue = 1;
        }

        protected override void SceneInit()
        {
            if (data == null)
            {
                data = CreateNewGraph();
            }

            CreateSceneRootForData();

            var sv = SceneView.lastActiveSceneView;
            Vector3 center = new Vector3(data.RangeX / 2, 0, data.RangeY / 2);
            sv.in2DMode = false;
            sv.LookAt(center, new Quaternion(1, 0, 0, 1), 200, true, false);
            sv.Repaint();
        }

        private void CreateSceneRootForData()
        {
            if (this.data == null)
            {
                return;
            }

            if (goRoot != null)
            {
                DestroyImmediate(goRoot);
            }

            goRoot = new GameObject("EditRoot");
            goRoot.transform.localPosition = Vector3.zero;
            goRoot.transform.localScale = Vector3.one;
            goRoot.transform.localRotation = Quaternion.identity;

            Shader shader = Shader.Find("AillieoUtils/TiledMapGrid");
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "quad";
            quad.transform.SetParent(goRoot.transform, false);
            quad.transform.localScale = new Vector3(data.RangeX, data.RangeY, 1);
            quad.transform.localPosition = new Vector3(data.RangeX / 2.0f, 0, data.RangeY / 2.0f);
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            texture = new Texture2D(data.RangeX, data.RangeY, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            Material material = new Material(shader);
            material.SetTexture("_MainTex", texture);
            Renderer renderer = quad.GetComponent<MeshRenderer>();
            renderer.material = material;

            for (int i = 0; i < data.RangeY; ++i)
            {
                for (int j = 0; j < data.RangeX; ++j)
                {
                    texture.SetPixel(i, j, data.Passable(i, j) ? Color.white : Color.black);
                }
            }
            texture.Apply();
        }

        private Color ValueToColor(float value)
        {
            return new Color(value, value, value, 1f);
        }

        private float ColorToValue(Color color)
        {
            return color.r;
        }

        protected override void SceneCleanUp()
        {
            if (goRoot != null)
            {
                DestroyImmediate(goRoot);
            }
        }

        protected override void OnSceneGUI(SceneView sceneView)
        {
            // Handles.Label(Vector3.zero, texture);

            Event current = Event.current;
            EventType et = current.type;

            bool pressed = false;
            if (current.isMouse)
            {
                pressed = (current.button == 0) && (et == EventType.MouseDrag || et == EventType.MouseDown);
            }

            if (pressed)
            {
                Vector2 mousePosition2D = GetMouseWorldPosition2D(current);
                Vector2Int pos = new Vector2Int((int)mousePosition2D.x, (int)mousePosition2D.y);
                if (pos.x >= 0 && pos.x < data.RangeX && pos.y >= 0 && pos.y < data.RangeY)
                {
                    Paint(pos.x, pos.y);
                    texture.Apply();
                }
            }

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }
        }

        private void Paint(int x, int y)
        {
            Color color = brushValue == 1 ? Color.black : Color.white;
            int xFrom = x - brushSize / 2;
            int yFrom = y - brushSize / 2;
            int xTo = xFrom + brushSize;
            int yTo = yFrom + brushSize;
            for (int i = xFrom; i < xTo; ++i)
            {
                for (int j = yFrom; j < yTo; ++j)
                {
                    texture.SetPixel(i, j, color);
                }
            }
        }
    }
}
