using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator.Editor
{

    public class NavMeshEditorWindow : BaseEditorWindow<NavMeshData>
    {
        [MenuItem("AillieoUtils/AStarPathfinding/NavMeshEditorWindow")]
        public static void Open()
        {
            GetWindowAndOpen<NavMeshEditorWindow>();
        }

        private Mesh mesh;

        public void OnGUI()
        {
            DrawSaveLoadButtons();


        }

        protected override void SceneInit()
        {

        }

        protected override void SceneCleanUp()
        {

        }

        protected override void OnSceneGUI(SceneView sceneView)
        {

        }
    }
}
