using System;

namespace AillieoUtils
{
    public delegate void PointChangedFunc(PointChangeInfo info);

    [Flags]
    public enum PointChangeFlag
    {
        None = 0,
        Add = 1 << 1,
        Remove = 1 << 2,
        OpenList = 1 << 3,
        ClosedList = 1 << 4,
    }

    public struct PointChangeInfo
    {
        public PointChangeInfo(Point point, PointChangeFlag flag)
        {
            this.point = point;
            changeFlag = flag;
        }

        public Point point;
        public PointChangeFlag changeFlag;
    }
}
