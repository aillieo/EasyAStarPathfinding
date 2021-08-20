using System;
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
            path = EditorPrefs.GetString(defaultPathKey, string.Empty);
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
            EditorPrefs.SetString(defaultPathKey, path);
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
            path = EditorGUILayout.TextField(new GUIContent("path"), path);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New"))
            {
                data = CreateNewGraph();
            }
            if (GUILayout.Button("Save"))
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = EditorUtility.SaveFilePanel("Where to save?", Application.dataPath, "data", "bytes");
                }

                Save();
            }
            if (GUILayout.Button("Load"))
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = EditorUtility.OpenFilePanel("Where to load?", Application.dataPath, "bytes");
                }

                Load();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        protected abstract void OnSceneGUI(SceneView sceneView);

        protected abstract void SceneInit();

        protected abstract void SceneCleanUp();

        private string defaultPathKey => $"DefaultPath{GetType().Name}";

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
