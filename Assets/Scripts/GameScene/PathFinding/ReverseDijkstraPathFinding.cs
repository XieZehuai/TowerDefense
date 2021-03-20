using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class ReverseDijkstraPathFinding : IPathFindingStrategy
    {
        protected int width;
        protected int height;
        protected PathNode[] nodes;

        public virtual void SetMapData(MapObject[,] datas)
        {
            width = datas.GetLength(0);
            height = datas.GetLength(1);
            nodes = new PathNode[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode node = new PathNode
                    {
                        x = x,
                        y = y,
                        index = GetIndex(x, y),
                        costG = int.MaxValue,
                        isWalkable = datas[x, y].IsWalkable(),
                        parentIndex = -1,
                    };

                    nodes[node.index] = node;
                }
            }
        }

        public virtual bool FindPaths(Vector2Int[] startPositions, Vector2Int endPosition, ref List<Vector2Int>[] paths)
        {
            PriorityQueue<PathNode> openList = new PriorityQueue<PathNode>(8); // 保存所有待寻路的节点（可用优先队列保存）
            HashSet<PathNode> closeList = new HashSet<PathNode>(); // 保存所有已经寻路过的节点

            int endIndex = GetIndex(endPosition);
            PathNode start = nodes[endIndex];
            start.costG = 0;
            nodes[endIndex] = start;
            openList.Add(start);

            while (!openList.IsEmpty)
            {
                PathNode currentNode = openList.DeleteMax();
                closeList.Add(currentNode);

                // 遍历当前节点所有相邻并且可行走的节点
                List<PathNode> neighbours = GetNeighbour(currentNode);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    PathNode neighbourNode = neighbours[i];

                    // 跳过已经搜索过的节点
                    if (closeList.Contains(neighbourNode)) continue;

                    int costG = currentNode.costG + GetDistance(currentNode, neighbourNode);

                    if (costG < neighbourNode.costG)
                    {
                        neighbourNode.costG = costG;
                        neighbourNode.parentIndex = currentNode.index;
                        nodes[neighbourNode.index] = neighbourNode;

                        // 没搜索过就加入openList，等待搜索
                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            for (int i = 0; i < startPositions.Length; i++)
            {
                if (!GetPath(startPositions[i], endPosition, ref paths[i]))
                {
                    ResetMapData();
                    return false;
                }
            }

            ResetMapData();
            return true;
        }

        protected int GetIndex(int x, int y)
        {
            return x * height + y;
        }

        protected int GetIndex(Vector2Int position)
        {
            return position.x * height + position.y;
        }

        protected PathNode GetNode(int x, int y)
        {
            return nodes[GetIndex(x, y)];
        }

        protected PathNode GetNode(Vector2Int position)
        {
            return nodes[GetIndex(position)];
        }

        protected bool IsPositionWalkable(int x, int y)
        {
            if (!IsValidPosition(new Vector2Int(x, y))) return false;

            return GetNode(x, y).isWalkable;
        }

        protected int GetDistance(PathNode a, PathNode b)
        {
            return GetDistance(new Vector2Int(a.x, a.y), new Vector2Int(b.x, b.y));
        }

        protected int GetDistance(Vector2Int a, Vector2Int b)
        {
            int h = Mathf.Abs(a.x - b.x);
            int v = Mathf.Abs(a.y - b.y);
            int remain = Mathf.Abs(h - v);
            return 14 * Mathf.Min(h, v) + 10 * remain;
        }

        // 判断节点位置是否有效
        protected bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
        }

        protected List<PathNode> GetNeighbour(PathNode node)
        {
            List<PathNode> neighbours = new List<PathNode>();

            int x = node.x;
            int y = node.y;

            bool left = IsPositionWalkable(x - 1, y);
            bool right = IsPositionWalkable(x + 1, y);
            bool up = IsPositionWalkable(x, y + 1);
            bool down = IsPositionWalkable(x, y - 1);

            bool leftUp = IsPositionWalkable(x - 1, y + 1);
            bool rightUp = IsPositionWalkable(x + 1, y + 1);
            bool leftDown = IsPositionWalkable(x - 1, y - 1);
            bool rightDown = IsPositionWalkable(x + 1, y - 1);

            if (left) neighbours.Add(GetNode(x - 1, y));
            if (right) neighbours.Add(GetNode(x + 1, y));
            if (up) neighbours.Add(GetNode(x, y + 1));
            if (down) neighbours.Add(GetNode(x, y - 1));

            if (left && up && leftUp) neighbours.Add(GetNode(x - 1, y + 1));
            if (right && up && rightUp) neighbours.Add(GetNode(x + 1, y + 1));
            if (left && down && leftDown) neighbours.Add(GetNode(x - 1, y - 1));
            if (right && down && rightDown) neighbours.Add(GetNode(x + 1, y - 1));

            return neighbours;
        }

        // 重置所有节点数据
        protected void ResetMapData()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                PathNode node = nodes[i];
                node.costG = int.MaxValue;
                node.parentIndex = -1;
                nodes[i] = node;
            }
        }

        protected bool GetPath(Vector2Int start, Vector2Int end, ref List<Vector2Int> path)
        {
            PathNode temp = nodes[GetIndex(start)];

            if (temp.parentIndex == -1) return false;

            path.Add(new Vector2Int(temp.x, temp.y));
            while (temp.parentIndex != -1)
            {
                temp = nodes[temp.parentIndex];
                path.Add(new Vector2Int(temp.x, temp.y));
            }

            return temp.x == end.x && temp.y == end.y;
        }
    }
}
