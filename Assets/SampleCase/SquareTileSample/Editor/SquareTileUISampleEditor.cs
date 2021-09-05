using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Samples
{
    [CustomEditor(typeof(SquareTileUISample))]
    public class SquareTileUISampleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(20);

            SquareTileUISample targetSquareTileUISample = target as SquareTileUISample;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            SerializedProperty assetFilePath = serializedObject.FindProperty("assetFilePath");
            EditorGUILayout.PropertyField(assetFilePath);
            if (GUILayout.Button("ChangeFilePath"))
            {
                assetFilePath.stringValue = EditorUtility.OpenFilePanel("Select a file?", ".", "bytes");
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();
        }
    }
}
