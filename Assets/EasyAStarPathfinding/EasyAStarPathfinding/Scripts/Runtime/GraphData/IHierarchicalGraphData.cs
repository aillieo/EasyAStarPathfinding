namespace AillieoUtils.Pathfinding
{
    public interface IHierarchicalGraphData<T> : IGraphData
    {
        int GetLevelCount();

        T GetNodeForLevel(int inputLevel, T inputNode, int targetLevel);

        void ConfigCurrentLevel(int curLevel);
    }
}
