using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{
    public abstract class BaseEditorWindow<T> : EditorWindow where T : IGraphData
    {
        protected T data;
        protected string path;

        protected static BaseEditorWindow<T> GetWindowAndOpen<U>() where U : BaseEditorWindow<T>
        {
            BaseEditorWindow<T> window = GetWindow<U>(typeof(U).Name);
            window.Show();
            return window;
        }

        protected virtual void OnEnable()
        {
            SceneInit();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        protected virtual void OnDisable()
        {
            SceneCleanUp();
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        protected virtual void Save()
        {
            SerializeHelper.Save(data, path);
        }

        protected virtual void Load()
        {
            data = SerializeHelper.Load<T>(path);
        }

        protected virtual T CreateNewGraph()
        {
            return Activator.CreateInstance<T>();
        }

        protected virtual void DrawSaveLoadButtons()
        {
            EditorGUILayout.BeginVertical("box");
            path = EditorGUILayout.TextField(new GUIContent("path"), $"{Application.dataPath}/data.bytes");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New"))
            {
                data = CreateNewGraph();
            }
            if (GUILayout.Button("Save"))
            {
                Save();
            }
            if (GUILayout.Button("Load"))
            {
                Load();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        protected abstract void OnSceneGUI(SceneView sceneView);

        protected abstract void SceneInit();

        protected abstract void SceneCleanUp();

        protected Vector2 GetMouseWorldPosition2D(Event evt)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            float Y = 0;
            float dist = (Y - mouseRay.origin.y) / mouseRay.direction.y;
            Vector2 mousePosition2D = mouseRay.GetPoint(dist).ToV2();
            return mousePosition2D;
        }
    }
}
