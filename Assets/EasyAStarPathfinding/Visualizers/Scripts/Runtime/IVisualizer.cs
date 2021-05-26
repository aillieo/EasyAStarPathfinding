namespace AillieoUtils.Pathfinding.Visualizers
{
    public interface IVisualizer
    {
        void DrawGraph(IGraphData graphData);
        void DrawContext(PathfindingContext context);
    }
}
