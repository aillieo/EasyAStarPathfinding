namespace AillieoUtils.Pathfinding.Visualizers
{
    public interface IVisualizer<T> where T : IGraphNode
    {
        void DrawGraph(IGraphData graphData);
        void DrawContext(PathfindingContext<T> context);
    }
}
