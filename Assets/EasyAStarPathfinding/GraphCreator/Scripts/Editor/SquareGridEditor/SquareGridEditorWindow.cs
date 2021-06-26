using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{

    public class SquareGridEditorWindow : BaseEditorWindow<SquareGridData>
    {
        [MenuItem("AillieoUtils/AStarPathfinding/SquareGridEditorWindow")]
        public static void Open()
        {
            GetWindowAndOpen<SquareGridEditorWindow>();
        }

        private Vector2Int dataRange;
        private GameObject goRoot;
        private Texture2D texture;
        private Texture2D savedTexture;

        public void OnGUI()
        {
            DrawSaveLoadButtons();

            savedTexture = EditorGUILayout.ObjectField(savedTexture, typeof(Texture2D), false) as Texture2D;
            GUILayout.Button("Save to texture");
            GUILayout.Button("Load from texture");
        }

        protected override void SceneInit()
        {
            if (data == null)
            {
                data = CreateNewGraph();
            }

            dataRange = new Vector2Int(10, 10);


            goRoot = new GameObject("EditRoot");
            goRoot.transform.localPosition = Vector3.zero;
            goRoot.transform.localScale = Vector3.one;
            goRoot.transform.localRotation = Quaternion.identity;

            Shader shader = Shader.Find("AillieoUtils/TiledMapGrid");
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "quad";
            quad.transform.SetParent(goRoot.transform, false);
            quad.transform.localPosition = new Vector3(dataRange.x / 2.0f, 0, dataRange.y / 2.0f);
            quad.transform.localScale = new Vector3(dataRange.x, dataRange.y, 1);
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            texture = new Texture2D(dataRange.x, dataRange.y, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            Material material = new Material(shader);
            material.SetTexture("_MainTex", texture);
            Renderer renderer = quad.GetComponent<MeshRenderer>();
            renderer.material = material;







            var sv = SceneView.lastActiveSceneView;
            Vector3 center = new Vector3(dataRange.x / 2, 0, dataRange.y / 2);
            sv.in2DMode = false;
            sv.LookAt(center, new Quaternion(1, 0, 0, 1), 200, true, false);
            sv.Repaint();
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

//                for (int i = 0; i < dataRange.y; ++i)
//                {
//                    for (int j = 0; j < dataRange.x; ++j)
//                    {
//                        texture.SetPixel(i, j, Color.black);
//                    }
//                }

                Vector2Int pos = new Vector2Int((int)mousePosition2D.x, (int)mousePosition2D.y);
                if (pos.x >= 0 && pos.x < dataRange.x && pos.y >= 0 && pos.y < dataRange.y)
                {
                    texture.SetPixel(pos.x, pos.y, Color.black);
                    //Debug.LogError($"{pos} | {dataRange}");
                    texture.Apply();
                }
            }

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }
        }
    }
}
