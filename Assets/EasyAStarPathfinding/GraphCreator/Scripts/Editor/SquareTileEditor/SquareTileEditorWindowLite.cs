using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{

    public class SquareTileEditorWindowLite : BaseEditorWindow<SquareTileMapData>
    {
        [MenuItem("AillieoUtils/AStarPathfinding/SquareTileEditorWindowLite")]
        public static void Open()
        {
            GetWindowAndOpen<SquareTileEditorWindowLite>();
        }

        private Vector2Int dataRange;
        private Texture2D texture;
        private Vector2 offset;
        private GUILayoutOption widthCtrl;

        private const int rangeMax = 2048;
        private const int rangeMin = 2;
        private const float buttonWidth = 20f;
        private const float barWidth = 30f;
        private const float viewWidthMax = 500f;

        public void OnGUI()
        {
            DrawSaveLoadButtons();

            EditorGUILayout.Space(10);

            DrawTileDataEditor();
        }

        private void DrawTileDataEditor()
        {
            if (data == null)
            {
                return;
            }

            dataRange.x = EditorGUILayout.IntSlider(dataRange.x, rangeMin, rangeMax);
            dataRange.y = EditorGUILayout.IntSlider(dataRange.y, rangeMin, rangeMax);
            if (GUILayout.Button("Resize"))
            {
                data.Resize(dataRange.x, dataRange.y);
                return;
            }

            int width = data.RangeX;
            int height = data.RangeY;
            width = Mathf.Clamp(width, rangeMin, rangeMax);
            height = Mathf.Clamp(height, rangeMin, rangeMax);

            // float viewWidthRaw = EditorGUIUtility.currentViewWidth;
            // viewWidthRaw = Mathf.Min(viewWidthRaw, viewWidthMax);
            // Rect r = GUILayoutUtility.GetRect(viewWidthRaw, viewWidthRaw);

            Rect viewRect = GUILayoutUtility.GetAspectRect(1, widthCtrl);
            float viewWidthRaw = viewRect.width;

            float viewWidth = viewWidthRaw - barWidth;

            float displayX = Mathf.Clamp(width, rangeMin, viewWidth / buttonWidth);
            float displayY = Mathf.Clamp(height, rangeMin, viewWidth / buttonWidth);

            if (width > displayX)
            {
                Rect sliderRect = new Rect(viewRect.position + new Vector2(barWidth, 0), new Vector2(viewRect.width - barWidth, barWidth));
                offset.x = GUI.HorizontalSlider(sliderRect, offset.x, 0, width - displayX);
            }
            else
            {
                offset.x = 0;
            }

            if (height > displayY)
            {
                Rect sliderRect = new Rect(viewRect.position + new Vector2(0, barWidth), new Vector2(barWidth, viewRect.height - barWidth));
                offset.y = GUI.VerticalSlider(sliderRect, offset.y, height - displayY, 0);
            }
            else
            {
                offset.y = 0;
            }

            Rect tileViewRect = new Rect(viewRect.position + new Vector2(barWidth, barWidth), new Vector2(viewRect.width - barWidth, viewRect.height - barWidth));
            Vector2 buttonSize = Vector2.one * buttonWidth;

            GUI.BeginGroup(tileViewRect, new GUIStyle("box"));
            int xStartIndex = (int)offset.x;
            int xEndIndex = (int)(offset.x + displayX);
            int yStartIndex = (int)offset.y;
            int yEndIndex = (int)(offset.y + displayY);

            for (int j = yEndIndex - 1; j >= yStartIndex; --j)
            {
                for (int i = xStartIndex; i < xEndIndex; ++i)
                {
                    bool passable = data.GetCost(i, j) < 0.5f;
                    Rect buttonRect = new Rect(new Vector2((i - offset.x) * buttonWidth, (offset.y + displayY - 1 - j) * buttonWidth), buttonSize);
                    if (GUI.Button(buttonRect, passable ? " " : "x"))
                    {
                        data.SetCost(i, j, passable ? 0 : 1);
                    }
                }
            }
            GUI.EndGroup();

            texture = EditorGUILayout.ObjectField(texture, typeof(Texture2D), false) as Texture2D;
            if (GUILayout.Button("Load from Texture"))
            {
                string err = null;
                if (TextureValid(texture, out err))
                {
                    TryLoadDataFromTexture(texture);
                }
                else
                {
                    Debug.LogError(err);
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            widthCtrl = GUILayout.MaxWidth(viewWidthMax);
        }

        private static bool TextureValid(Texture2D tex, out string message)
        {
            message = null;

            if (tex == null)
            {
                message = "texture is null";
                return false;
            }

            if (!tex.isReadable)
            {
                message = "texture is not readable";
                return false;
            }

            if (tex.width > rangeMax || tex.height > rangeMax)
            {
                message = $"too large size (should <= {rangeMax})";
                return false;
            }

            if (tex.width < rangeMin || tex.height < rangeMin)
            {
                message = $"too small size (should >= {rangeMin})";
                return false;
            }

            if (tex.filterMode != FilterMode.Point)
            {
                message = $"should use FilterMode: {FilterMode.Point}";
                return false;
            }

            if (tex.format != TextureFormat.R8)
            {
                message = $"should use TextureFormat: {TextureFormat.R8}";
                return false;
            }

            return true;
        }

        private bool TryLoadDataFromTexture(Texture2D tex)
        {
            if (data == null)
            {
                return false;
            }

            int width = Mathf.Min(tex.width, rangeMax);
            int height = Mathf.Min(tex.height, rangeMax);

            data.Resize(width, height);
            Color[] colors = tex.GetPixels();

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    Color c = colors[i + j * width];
                    data.SetCost(i, j, c.r);
                }
            }

            return true;
        }

        protected override void OnSceneGUI(SceneView sceneView)
        {
        }

        protected override void SceneInit()
        {
        }

        protected override void SceneCleanUp()
        {
        }
    }
}
