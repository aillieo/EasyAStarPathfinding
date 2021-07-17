using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Samples
{
    [CustomEditor(typeof(SquareGridSample))]
    public class SquareGridSampleEditor : Editor
    {
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(20);

            SquareGridSample targetPathfindingTest = target as SquareGridSample;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            SerializedProperty assetFilePath = serializedObject.FindProperty("assetFilePath");
            EditorGUILayout.PropertyField(assetFilePath);
            if (GUILayout.Button("ChangeFilePath"))
            {
                assetFilePath.stringValue = EditorUtility.OpenFilePanel("Select a file?", ".", "bytes");
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("FindPath"))
            {
                targetPathfindingTest.FindPath();
            }

            if (GUILayout.Button("FindPathAsync"))
            {
                targetPathfindingTest.FindPathAsync();
            }

            if (GUILayout.Button("FindPathInCoroutine"))
            {
                targetPathfindingTest.FindPathInCoroutine();
            }
        }
    }
}
