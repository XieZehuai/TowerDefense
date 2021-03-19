using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace TowerDefense
{
    public struct Node
    {
        public int x;
        public int y;
        public int index;

        public int costG;
        public int costH;
        public int costF => costG = costH;

        public bool isWalkable;
        public int parentIndex;
    }


    public class PathFinder
    {
        private int width;
        private int height;
        private Vector2Int endPos;
        private List<Node> nodes;

        /// <summary>
        /// 设置地图数据
        /// </summary>
        /// <param name="width">地图宽度</param>
        /// <param name="height">地图高度</param>
        /// <param name="datas">原始数据</param>
        /// <param name="endPos">目标点</param>
        public void SetMapData(int width, int height, MapObject[,] datas, Vector2Int endPos)
        {
            this.width = width;
            this.height = height;
            this.endPos = endPos;

            nodes = new List<Node>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Node node = new Node
                    {
                        x = x,
                        y = y,
                        index = GetIndex(x, y),
                        costG = int.MaxValue,
                        costH = GetDistance(new Vector2Int(x, y), new Vector2Int(endPos.x, endPos.y)),
                        isWalkable = IsWalkable(datas[x, y]),
                        parentIndex = -1,
                    };

                    nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 搜索路径
        /// </summary>
        /// <param name="startPosArray">由所有起始点组成的数组</param>
        /// <param name="getPath">寻路成功时是否获取路径</param>
        /// <param name="paths">保存路径</param>
        /// <returns>true寻路成功，false寻路失败</returns>
        public bool FindPaths(Vector2Int[] startPosArray, bool getPath, ref List<Vector2Int>[] paths)
        {
            float startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < startPosArray.Length; i++)
            {
                if (!AStar(startPosArray[i], getPath, ref paths[i]))
                {
                    return false;
                }
            }

            Debug.Log("寻路时间：" + (Time.realtimeSinceStartup - startTime) * 1000f + "毫秒");

            return true;
        }

        private bool AStar(Vector2Int startPos, bool getPath, ref List<Vector2Int> path)
        {
            // 设置起始节点costG为0
            Node start = nodes[GetIndex(startPos.x, startPos.y)];
            start.costG = 0;
            nodes[start.index] = start;

            // 计算目标节点的索引
            int endIndex = GetIndex(endPos.x, endPos.y);

            List<int> openList = new List<int>(); // 保存所有待搜索节点的索引
            HashSet<int> closeList = new HashSet<int>(); // 保存所有已搜索节点的索引

            openList.Add(start.index);

            while (openList.Count > 0)
            {
                // 从待搜索节点中选取costF最小的节点的索引
                int currIndex = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //if (nodes[openList[i]].costH < nodes[currIndex].costH && nodes[openList[i]].costF < nodes[currIndex].costF)
                    if (nodes[openList[i]].costG < nodes[currIndex].costG)
                    {
                        currIndex = openList[i];
                    }
                }

                // 抵达终点
                if (currIndex == endIndex) break;

                Node currNode = nodes[currIndex];

                // 把当前结点从open list移除
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i] == currIndex)
                    {
                        openList.QuickRemove(i);
                        break;
                    }
                }

                closeList.Add(currIndex);

                var neighbours = GetNeighbour(currNode);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    Node neighbourNode = neighbours[i];
                    if (closeList.Contains(neighbourNode.index)) continue; // 跳过已经搜索过的节点

                    int g = currNode.costG + GetDistance(new Vector2Int(currNode.x, currNode.y), new Vector2Int(neighbourNode.x, neighbourNode.y));

                    if (g < neighbourNode.costG)
                    {
                        neighbourNode.parentIndex = currIndex;
                        neighbourNode.costG = g;
                        nodes[neighbourNode.index] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            Node endNode = nodes[GetIndex(endPos.x, endPos.y)];
            bool isSuccess;

            if (endNode.parentIndex == -1)
            {
                isSuccess = false;
            }
            else
            {
                isSuccess = true;

                if (getPath)
                {
                    Stack<Vector2Int> stack = new Stack<Vector2Int>();
                    stack.Push(endPos);

                    Node temp = endNode;
                    while (temp.parentIndex != -1)
                    {
                        temp = nodes[temp.parentIndex];
                        stack.Push(new Vector2Int(temp.x, temp.y));
                    }

                    path = stack.ToList();
                }
            }

            ResetNodes();

            return isSuccess;
        }

        // 获取所有与当前节点相邻并且可抵达的节点
        private List<Node> GetNeighbour(Node node)
        {
            List<Node> neighbours = new List<Node>();

            int x = node.x;
            int y = node.y;

            bool left = IsNeighbourWalkable(x - 1, y);
            bool right = IsNeighbourWalkable(x + 1, y);
            bool up = IsNeighbourWalkable(x, y + 1);
            bool down = IsNeighbourWalkable(x, y - 1);

            bool leftUp = IsNeighbourWalkable(x - 1, y + 1);
            bool rightUp = IsNeighbourWalkable(x + 1, y + 1);
            bool leftDown = IsNeighbourWalkable(x - 1, y - 1);
            bool rightDown = IsNeighbourWalkable(x + 1, y - 1);

            if (left) neighbours.Add(nodes[GetIndex(x - 1, y)]);
            if (right) neighbours.Add(nodes[GetIndex(x + 1, y)]);
            if (up) neighbours.Add(nodes[GetIndex(x, y + 1)]);
            if (down) neighbours.Add(nodes[GetIndex(x, y - 1)]);

            if (left && up && leftUp) neighbours.Add(nodes[GetIndex(x - 1, y + 1)]);
            if (right && up && rightUp) neighbours.Add(nodes[GetIndex(x + 1, y + 1)]);
            if (left && down && leftDown) neighbours.Add(nodes[GetIndex(x - 1, y - 1)]);
            if (right && down && rightDown) neighbours.Add(nodes[GetIndex(x + 1, y - 1)]);

            return neighbours;
        }

        // 判断节点是否可走
        private bool IsWalkable(MapObject obj)
        {
            MapObjectType type = obj.type;

            return type == MapObjectType.Road || type == MapObjectType.Destination || type == MapObjectType.SpawnPoint;
        }

        // 判断相邻节点是否可走
        private bool IsNeighbourWalkable(int x, int y)
        {
            if (!IsValidPos(new Vector2Int(x, y))) return false;

            return nodes[GetIndex(x, y)].isWalkable;
        }

        // 获取节点索引
        private int GetIndex(int x, int y)
        {
            return x * height + y;
        }

        // 获取两个节点之间的距离
        private int GetDistance(Vector2Int a, Vector2Int b)
        {
            int h = Mathf.Abs(a.x - b.x);
            int v = Mathf.Abs(a.y - b.y);
            int remain = Mathf.Abs(h - v);
            return 14 * Mathf.Min(h, v) + 10 * remain;
        }

        // 判断节点位置是否有效
        private bool IsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        // 重置所有节点数据
        private void ResetNodes()
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                Node node = nodes[j];
                node.costG = int.MaxValue;
                node.costH = GetDistance(new Vector2Int(node.x, node.y), new Vector2Int(endPos.x, endPos.y));
                node.parentIndex = -1;
                nodes[j] = node;
            }
        }
    }
}
