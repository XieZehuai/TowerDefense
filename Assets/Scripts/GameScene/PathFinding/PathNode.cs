using System;
using System.Collections.Generic;

namespace TowerDefense
{
    public struct PathNode : IComparable<PathNode>
    {
        public int x;
        public int y;
        public int index;

        public int costG;
        public int costH;
        public int costF => costG = costH;

        public bool isWalkable;
        public int parentIndex;

        public int CompareTo(PathNode other)
        {
            return other.costG - costG;
        }

        public override int GetHashCode()
        {
            return x << 16 + y;
        }
    }
}
