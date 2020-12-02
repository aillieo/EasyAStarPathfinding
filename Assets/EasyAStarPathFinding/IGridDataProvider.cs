namespace AillieoUtils
{
    public interface IGridDataProvider
    {
        bool this[int x, int y] { get; }
    }
}
