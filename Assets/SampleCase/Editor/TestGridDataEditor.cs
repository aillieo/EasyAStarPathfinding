using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestGridData))]
public class TestGridDataEditor : Editor
{
    public static GUILayoutOption[] widthControl_20;
    private SerializedProperty rangeX;
    private SerializedProperty rangeY;
    private SerializedProperty data;

    private Texture2D texture;
    private Vector2 offset;

    private void OnEnable()
    {
        rangeX = serializedObject.FindProperty("rangeX");
        rangeY = serializedObject.FindProperty("rangeY");
        data = serializedObject.FindProperty("data");
        widthControl_20 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(20) };
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(rangeX);
        EditorGUILayout.PropertyField(rangeY);
        if(EditorGUI.EndChangeCheck())
        {
            int newSize = rangeX.intValue * rangeY.intValue;
            data.arraySize = newSize;
        }
        int width = rangeX.intValue;
        int height = rangeY.intValue;
        width = Mathf.Clamp(width, 2, 2048);
        height = Mathf.Clamp(height, 2, 2048);

        int displayX = Mathf.Clamp(width, 2, 10);
        int displayY = Mathf.Clamp(height, 2, 10);

//        offset.x = EditorGUILayout.Slider(offset.x, 0, width - displayX);
//        offset.y = EditorGUILayout.Slider(offset.y, 0, height - displayY);

//        for (int j = (int)offset.y + displayY - 1; j >= (int)offset.y; --j)
//        {
//            EditorGUILayout.BeginHorizontal();
//            for (int i = (int)offset.x; i < (int)offset.x + displayX; ++i)
//            {
//                SerializedProperty p = data.GetArrayElementAtIndex(i + j * width);
//                bool v = p.boolValue;
//                if (GUILayout.Button(v ? " " : "x", widthControl_20))
//                {
//                    p.boolValue = !v;
//                }
//            }
//            EditorGUILayout.EndHorizontal();
//        }




        // float w = this.Position.width;
        float buttonWidth = 20f;
        float barWidth = 30f;
        float w = EditorGUIUtility.currentViewWidth - 30f;
        w = Mathf.Min(w, 500f);
        Rect r = GUILayoutUtility.GetRect(w, w);
        r.width = w;
        r.height = r.width;
        float viewWidth = w - barWidth;

        displayX = Mathf.Clamp(width, 2, (int)(viewWidth / buttonWidth));
        displayY = Mathf.Clamp(height, 2, (int)(viewWidth / buttonWidth));

        offset.x = GUI.HorizontalSlider(new Rect(r.position + new Vector2(barWidth, 0), new Vector2(r.width - barWidth, barWidth)), offset.x, 0, width - displayX);
        offset.y = GUI.VerticalSlider(new Rect(r.position + new Vector2(0, barWidth), new Vector2(barWidth, r.height - barWidth)), offset.y, height - displayY, 0);

        GUI.BeginGroup(new Rect(r.position + new Vector2(barWidth, barWidth), new Vector2(r.width - barWidth, r.height - barWidth)), new GUIStyle("box"));
        for (int j = (int)offset.y + displayY - 1; j >= (int)offset.y; --j)
        {
            for (int i = (int)offset.x; i < (int)offset.x + displayX; ++i)
            {
                SerializedProperty p = data.GetArrayElementAtIndex(i + j * width);
                bool v = p.boolValue;
                if (GUI.Button(new Rect(new Vector2((i - offset.x) * 20, (- j + offset.y + displayY - 1) * 20), new Vector2(20, 20)), v ? " " : "x"))
                {
                    p.boolValue = !v;
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

        if (tex.width > 2048 || tex.height > 2048)
        {
            message = "too large size (should <= 2048)";
        }

        if (tex.filterMode != FilterMode.Point)
        {
            message = "should use FilterMode.Point";
            return false;
        }

        if (tex.format != TextureFormat.R8)
        {
            message = "should use TextureFormat.R8";
            return false;
        }

        return true;
    }

    private bool TryLoadDataFromTexture(Texture2D tex)
    {
        TestGridData dataObj = target as TestGridData;
        int width = Mathf.Min(tex.width, 2048);
        int height = Mathf.Min(tex.height, 2048);

        dataObj.rangeX = width;
        dataObj.rangeY = height;
        Color[] colors = tex.GetPixels();

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Color c = colors[i + j * width];
                //Debug.LogError($"{i},{j},{i + j * width} --> c = {c}");
                //Color c = tex.GetPixel((float)i / width, (float)j / height);
                dataObj[i, j] = c.r > 0.5f;
            }
        }

        //Color c1 = tex.GetPixel(0, 0);
        //Debug.LogError($"c1 = {c1}");

        EditorUtility.SetDirty(dataObj);

        return true;
    }
}
