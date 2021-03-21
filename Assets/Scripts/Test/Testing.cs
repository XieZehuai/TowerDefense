using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        private int width = 20;
        private int height = 20;
        private int testTimes = 20;

        private PathNode[,] nodes;

        private PathFindingTest finder;
        private NativeArray<int2> starts;
        private NativeList<int2>[] paths;
        private float timer = 0f;
        private MapObject[,] datas;
        private int2 end;

        private void Start()
        {
            finder = new PathFindingTest();

            nodes = new PathNode[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode node = new PathNode
                    {
                        x = x,
                        y = y,
                        distance = int.MaxValue,
                        isWalkable = true
                    };

                    nodes[x, y] = node;
                }
            }

            end = new int2(width - 1, height - 1);
            datas = new MapObject[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    datas[i, j] = new MapObject(MapObjectType.Road, i, j);
                }
            }

            starts = new NativeArray<int2>(testTimes, Allocator.Persistent);
            for (int i = 0; i < starts.Length; i++)
            {
                starts[i] = new int2(0, 0);
            }

            paths = new NativeList<int2>[starts.Length];
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= 1f)
            {
                Debug.Log("===============================");
                timer = 0f;
                finder.Init(width, height, datas, end);
                finder.FindPaths(starts, ref paths);

                float startTime = Time.realtimeSinceStartup;

                FindPath(new Vector2Int(width - 1, height - 1));

                Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f);
            }
        }

        private void FindPath(Vector2Int endPos)
        {
            PathNode start = nodes[endPos.x, endPos.y];
            start.distance = 0;
            PriorityQueue<PathNode> openList = new PriorityQueue<PathNode>(new PriorityComparer()); // 保存所有待寻路的节点（可用优先队列保存）
            HashSet<PathNode> closeList = new HashSet<PathNode>(); // 保存所有已经寻路过的节点

            openList.Add(start);
            while (!openList.IsEmpty)
            {
                PathNode currNode = openList.DeleteMax();
                closeList.Add(currNode);

                // 遍历当前节点所有相邻并且可行走的节点
                foreach (PathNode neighbourNode in GetNeighbour(currNode))
                {
                    // 跳过已经搜索过的节点
                    if (closeList.Contains(neighbourNode)) continue;

                    int distance = currNode.distance + GetDistance(currNode, neighbourNode);

                    if (distance < neighbourNode.distance)
                    {
                        neighbourNode.distance = distance;
                        neighbourNode.parent = currNode;

                        // 没搜索过就加入openList，等待搜索
                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            ResetNodes();
        }

        private void ResetNodes()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y].parent = nodes[x, y];
                    nodes[x, y].distance = int.MaxValue;
                }
            }
        }

        private List<PathNode> GetNeighbour(PathNode node)
        {
            List<PathNode> neighbours = new List<PathNode>();

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

            if (left) neighbours.Add(nodes[x - 1, y]);
            if (right) neighbours.Add(nodes[x + 1, y]);
            if (up) neighbours.Add(nodes[x, y + 1]);
            if (down) neighbours.Add(nodes[x, y - 1]);

            if (left && up && leftUp) neighbours.Add(nodes[x - 1, y + 1]);
            if (right && up && rightUp) neighbours.Add(nodes[x + 1, y + 1]);
            if (left && down && leftDown) neighbours.Add(nodes[x - 1, y - 1]);
            if (right && down && rightDown) neighbours.Add(nodes[x + 1, y - 1]);

            return neighbours;
        }

        private bool IsNeighbourWalkable(int x, int y)
        {
            if (!IsValidPos(new Vector2Int(x, y))) return false;

            return nodes[x, y].isWalkable;
        }

        private bool IsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        private int GetDistance(PathNode a, PathNode b)
        {
            int h = Mathf.Abs(a.x - b.x);
            int v = Mathf.Abs(a.y - b.y);
            int remain = Mathf.Abs(h - v);
            return 14 * Mathf.Min(h, v) + 10 * remain;
        }
    }


    public class PriorityComparer : IPriorityComparer<PathNode>
    {
        public int Compare(PathNode a, PathNode b)
        {
            return b.distance - a.distance;
        }
    }


    public class PathNode
    {
        public int x;
        public int y;

        public bool hasSearch;
        public int distance;
        public bool isWalkable;
        public PathNode parent;
    }
}
