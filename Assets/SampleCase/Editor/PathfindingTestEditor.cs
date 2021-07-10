using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingTest))]
public class PathfindingTestEditor : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PathfindingTest targetPathfindingTest = target as PathfindingTest;

        if (GUILayout.Button("LoadData"))
        {
            //string dataPath = EditorUtility.OpenFilePanel("Select grid data file:", Application.dataPath, "bytes");
            string dataPath = Application.dataPath + "/data.bytes";
            targetPathfindingTest.LoadData(dataPath);
        }

        if(targetPathfindingTest.gridData != null)
        {
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

            if (GUILayout.Button("FindPathInCoroutineV2"))
            {
                targetPathfindingTest.FindPathInCoroutineV2();
            }
        }
    }
}
