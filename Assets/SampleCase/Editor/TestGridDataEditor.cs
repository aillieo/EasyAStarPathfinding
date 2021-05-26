using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestGridData))]
public class TestGridDataEditor : Editor
{
    private Vector2Int dataRange;
    private Texture2D texture;
    private Vector2 offset;
    private GUILayoutOption widthCtrl;

    private const int rangeMax = 2048;
    private const int rangeMin = 2;
    private const float buttonWidth = 20f;
    private const float barWidth = 30f;
    private const float viewWidthMax = 500f;

    private void OnEnable()
    {
        TestGridData testGridData = target as TestGridData;
        dataRange.x = testGridData.RangeX;
        dataRange.y = testGridData.RangeY;

        widthCtrl = GUILayout.MaxWidth(viewWidthMax);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TestGridData testGridData = target as TestGridData;

        dataRange.x = EditorGUILayout.IntSlider(dataRange.x, rangeMin, rangeMax);
        dataRange.y = EditorGUILayout.IntSlider(dataRange.y, rangeMin, rangeMax);
        if (GUILayout.Button("Resize"))
        {
            testGridData.Resize(dataRange.x, dataRange.y);
            EditorUtility.SetDirty(target);
            return;
        }

        int width = testGridData.RangeX;
        int height = testGridData.RangeY;
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

        Rect gridViewRect = new Rect(viewRect.position + new Vector2(barWidth, barWidth), new Vector2(viewRect.width - barWidth, viewRect.height - barWidth));
        Vector2 buttonSize = Vector2.one * buttonWidth;

        GUI.BeginGroup(gridViewRect, new GUIStyle("box"));
        int xStartIndex = (int)offset.x;
        int xEndIndex = (int)(offset.x + displayX);
        int yStartIndex = (int)offset.y;
        int yEndIndex = (int)(offset.y + displayY);

        for (int j = yEndIndex - 1; j >= yStartIndex; --j)
        {
            for (int i = xStartIndex; i < xEndIndex; ++i)
            {
                bool v = (target as TestGridData).Passable(i, j);
                Rect buttonRect = new Rect(new Vector2((i - offset.x) * buttonWidth, (offset.y + displayY - 1 - j) * buttonWidth), buttonSize);
                if (GUI.Button(buttonRect, v ? " " : "x"))
                {
                    (target as TestGridData).SetPassable(i, j, !v);
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

        serializedObject.ApplyModifiedProperties();

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
        TestGridData dataObj = target as TestGridData;
        int width = Mathf.Min(tex.width, rangeMax);
        int height = Mathf.Min(tex.height, rangeMax);

        dataObj.Resize(width, height);
        Color[] colors = tex.GetPixels();

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Color c = colors[i + j * width];
                dataObj.SetPassable(i, j, c.r > 0.5f);
            }
        }

        EditorUtility.SetDirty(dataObj);

        return true;
    }
}
