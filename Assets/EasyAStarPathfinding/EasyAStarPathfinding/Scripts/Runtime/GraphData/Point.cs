namespace AillieoUtils.Pathfinding
{
    public class Point : IGraphNode
    {
        public int id { get; private set; }

        public int flag { get; private set; }

        public float cost { get; private set; }
    }
}
