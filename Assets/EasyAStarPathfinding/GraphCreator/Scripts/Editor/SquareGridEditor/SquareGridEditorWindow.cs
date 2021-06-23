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

        public void OnGUI()
        {
            DrawSaveLoadButtons();
        }

        protected override void SceneInit()
        {
            if (data == null)
            {
                data = CreateNew();
            }



            var range = new Vector2Int(10, 10);


            goRoot = new GameObject("EditRoot");
            goRoot.transform.localPosition = Vector3.zero;
            goRoot.transform.localScale = Vector3.one;
            goRoot.transform.localRotation = Quaternion.identity;

            Shader shader = Shader.Find("AillieoUtils/TiledMapGrid");
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "quad";
            quad.transform.SetParent(goRoot.transform, false);
            quad.transform.localPosition = new Vector3(range.x / 2.0f, 0, range.y / 2.0f);
            quad.transform.localScale = new Vector3(range.x, range.y, 1);
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            texture = new Texture2D(range.x, range.y, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            Material material = new Material(shader);
            material.SetTexture("_MainTex", texture);
            Renderer renderer = quad.GetComponent<MeshRenderer>();
            renderer.material = material;







            var sv = SceneView.lastActiveSceneView;
            Vector3 center = new Vector3(range.x / 2, 0, range.y / 2);
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
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = Screen.height - mousePos.y - 40;
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    Vector2 pos = new Vector2(hitInfo.point.x, hitInfo.point.z);

                    for (int i = 0; i < dataRange.y; ++i)
                    {
                        for (int j = 0; j < dataRange.x; ++j)
                        {
                            texture.SetPixel(i, j, Color.black);
                        }
                    }

                    texture.SetPixel((int)pos.x, (int)pos.y, Color.black);
                    Debug.LogError($"hit {pos}");
                }

                texture.Apply();
            }

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }
        }
    }
}
