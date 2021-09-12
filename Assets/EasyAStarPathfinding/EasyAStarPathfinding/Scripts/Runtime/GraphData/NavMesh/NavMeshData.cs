using System;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class NavMeshData : INavMeshData
    {
        public class NMFace : IGraphNode
        {
            public Vector2 center;
            public int[] verts;

            public int id { get; private set; }

            public int flag { get; private set; }

            public float cost { get; private set; }
        }

        public class NMVertex
        {
            public Tile position;
            public int index;
        }

        // mesh data
        private Vector2[] verts;
        private int[] triangles;

        // 重新组织一下
        private NMFace[] faces;
        private Dictionary<int, int[]> connections;
        //private RTree<NMFace> faceTree;

        public IEnumerable<NMFace> CollectNeighbor(NMFace current)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NMFace> GetAllNodes()
        {
            throw new NotImplementedException();
        }
    }
}
