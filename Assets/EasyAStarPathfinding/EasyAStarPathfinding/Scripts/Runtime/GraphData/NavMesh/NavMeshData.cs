using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class NavMeshData : INavMeshData
    {
        public class NMFace
        {
            public int index;
            public Point center;
            public int[] verts;
        }

        public class NMVertex
        {
            public Point position;
            public int index;
        }

        // mesh data
        private Point[] verts;
        private int[] triangles;

        // 重新组织一下
        private NMFace[] faces;
        private Dictionary<int, int[]> connections;
        //private RTree<NMFace> faceTree;

        public IEnumerable<NMFace> CollectNeighbor(NMFace current)
        {
            throw new NotImplementedException();
        }
    }
}
