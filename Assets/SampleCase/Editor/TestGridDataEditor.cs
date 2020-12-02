using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestGridData))]
public class TestGridDataEditor : Editor
{
    public static GUILayoutOption[] widthControl_50;
    SerializedProperty rangeX;
    SerializedProperty rangeY;
    SerializedProperty data;

    private void OnEnable()
    {
        rangeX = serializedObject.FindProperty("rangeX");
        rangeY = serializedObject.FindProperty("rangeY");
        data = serializedObject.FindProperty("data");
        widthControl_50 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(20) };
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
        int x = rangeX.intValue;
        int y = rangeY.intValue;

        x = Mathf.Min(x, 10);
        y = Mathf.Min(y, 10);

        for (int j = y - 1; j >= 0; --j)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < x; ++i)
            {
                SerializedProperty p = data.GetArrayElementAtIndex(i + j * x);
                bool v = p.boolValue;
                if (GUILayout.Button(v ? " " : "x", widthControl_50))
                {
                    p.boolValue = !v;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
