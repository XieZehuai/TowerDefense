using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TowerDefense.Test
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


    public class CNode
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


    public class PathFindingTest
    {
        private int width;
        private int height;
        private int2 endPos;
        private NativeArray<Node> nodes;
        private List<CNode> cnodes;

        public void Init(int width, int height, MapObject[,] datas, int2 endPos)
        {
            this.width = width;
            this.height = height;
            this.endPos = endPos;
            nodes = CreateMap(datas);

            cnodes = new List<CNode>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CNode cnode = new CNode
                    {
                        x = x,
                        y = y,
                        index = CGetIndex(x, y),
                        costG = int.MaxValue,
                        costH = CGetDistance(new Vector2Int(x, y), new Vector2Int(endPos.x, endPos.y)),
                        isWalkable = CIsWalkable(datas[x, y]),
                        parentIndex = -1,
                    };

                    cnodes.Add(cnode);
                }
            }
        }

        private NativeArray<Node> CreateMap(MapObject[,] datas)
        {
            NativeArray<Node> arr = new NativeArray<Node>(width * height, Allocator.TempJob);

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
                        costH = GetDistance(new int2(x, y), endPos),
                        isWalkable = IsWalkable(datas[x, y]),
                        parentIndex = -1,
                    };

                    arr[node.index] = node;
                }
            }

            return arr;
        }

        public bool FindPaths(NativeArray<int2> startPosArray, ref NativeList<int2>[] paths)
        {
            float startTime = Time.realtimeSinceStartup;

            int startPosCount = startPosArray.Length;
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(startPosCount, Allocator.TempJob);
            FindPathJob[] jobs = new FindPathJob[startPosCount];

            for (int i = 0; i < startPosCount; i++)
            {
                jobs[i] = new FindPathJob
                {
                    index = i,
                    width = width,
                    height = height,
                    startPos = startPosArray[i],
                    endPos = endPos,
                    nodes = new NativeArray<Node>(nodes, Allocator.TempJob),
                };

                jobHandles[i] = jobs[i].Schedule();
            }

            JobHandle.CompleteAll(jobHandles);
            Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f);
            jobHandles.Dispose();

            bool success = true;
            for (int i = 0; i < startPosCount; i++)
            {
                jobs[i].Dispose();
            }

            startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < startPosArray.Length; i++)
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                if (!FindPath(startPosArray[i], false, ref path))
                {
                    return false;
                }

                paths[i] = path;
            }

            Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f);

            startTime = Time.realtimeSinceStartup;
            for (int i = 0; i < startPosArray.Length; i++)
            {
                CFindPath(new Vector2Int(startPosArray[i].x, startPosArray[i].y));
            }
            Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f);

            nodes.Dispose();
            return success;
        }

        #region JOBS
        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int index;
            public int width;
            public int height;
            public int2 startPos;
            public int2 endPos;
            public NativeArray<Node> nodes;

            public void Execute()
            {
                Node start = nodes[GetIndex(startPos.x, startPos.y)];
                start.costG = 0;
                nodes[start.index] = start;
                int endIndex = GetIndex(endPos.x, endPos.y);

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closeList = new NativeList<int>(Allocator.Temp);

                openList.Add(start.index);
                while (openList.Length > 0)
                {
                    int currIndex = openList[0];
                    for (int i = 1; i < openList.Length; i++)
                    {
                        if (nodes[openList[i]].costF < nodes[currIndex].costF)
                        {
                            currIndex = openList[i];
                        }
                    }

                    // 抵达终点
                    if (currIndex == endIndex) break;

                    Node currNode = nodes[currIndex];

                    // 把当前结点从open list移除
                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closeList.Add(currIndex);

                    NativeList<Node> neighbours = GetNeighbours(currNode);
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        Node neighbourNode = neighbours[i];
                        if (closeList.Contains(neighbourNode.index)) continue; // 已经加入关闭列表

                        int g = currNode.costG + GetDistance(new int2(currNode.x, currNode.y), new int2(neighbourNode.x, neighbourNode.y));

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

                    neighbours.Dispose();
                }
            }

            public void Dispose()
            {
                nodes.Dispose();
            }

            private NativeList<Node> GetNeighbours(Node node)
            {
                NativeList<Node> neighbours = new NativeList<Node>(Allocator.Temp);

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

            private bool IsNeighbourWalkable(int x, int y)
            {
                if (!IsValidPos(new int2(x, y))) return false;

                return nodes[GetIndex(x, y)].isWalkable;
            }

            private int GetIndex(int x, int y)
            {
                return x * height + y;
            }

            private int GetDistance(int2 a, int2 b)
            {
                int h = math.abs(a.x - b.x);
                int v = math.abs(a.y - b.y);
                int remain = math.abs(h - v);
                return 14 * math.min(h, v) + 10 * remain;
            }

            private bool IsValidPos(int2 pos)
            {
                return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
            }

            private NativeList<int2> GetPath(Node endNode)
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);

                if (endNode.parentIndex != -1)
                {
                    Node temp = endNode;
                    while (temp.parentIndex != -1)
                    {
                        path.Add(new int2(temp.x, temp.y));
                        temp = nodes[temp.parentIndex];
                    }

                    path.Add(new int2(temp.x, temp.y));
                }

                return path;
            }
        }
        #endregion

        #region DOTS
        public bool FindPath(int2 startPos, bool getPath, ref NativeList<int2> path)
        {
            Node start = nodes[GetIndex(startPos.x, startPos.y)];
            start.costG = 0;
            nodes[start.index] = start;
            int endIndex = GetIndex(endPos.x, endPos.y);

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closeList = new NativeList<int>(Allocator.Temp);

            openList.Add(start.index);
            while (openList.Length > 0)
            {
                int currIndex = openList[0];
                for (int i = 1; i < openList.Length; i++)
                {
                    if (nodes[openList[i]].costF < nodes[currIndex].costF)
                    {
                        currIndex = openList[i];
                    }
                }

                // 抵达终点
                if (currIndex == endIndex) break;

                Node currNode = nodes[currIndex];

                // 把当前结点从open list移除
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closeList.Add(currIndex);

                //for (int i = 0; i < neighbours.Length; i++)
                NativeList<Node> neighbours = GetNeighbours(nodes, currNode);
                for (int i = 0; i < neighbours.Length; i++)
                {
                    Node neighbourNode = neighbours[i];
                    if (closeList.Contains(neighbourNode.index)) continue; // 已经加入关闭列表

                    int g = currNode.costG + GetDistance(new int2(currNode.x, currNode.y), new int2(neighbourNode.x, neighbourNode.y));

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

            Node endNode = nodes[endIndex];
            if (endNode.parentIndex == -1) // 没找到路径
            {
                ResetNodes();
                return false;
            }
            else // 找到路径
            {
                if (getPath)
                {
                    GetPath(endNode, ref path);
                }

                ResetNodes();
                return true;
            }
        }

        private NativeList<Node> GetNeighbours(NativeArray<Node> nodes, Node node)
        {
            NativeList<Node> neighbours = new NativeList<Node>(Allocator.Temp);

            int x = node.x;
            int y = node.y;

            bool left = IsNeighbourWalkable(nodes, x - 1, y);
            bool right = IsNeighbourWalkable(nodes, x + 1, y);
            bool up = IsNeighbourWalkable(nodes, x, y + 1);
            bool down = IsNeighbourWalkable(nodes, x, y - 1);

            bool leftUp = IsNeighbourWalkable(nodes, x - 1, y + 1);
            bool rightUp = IsNeighbourWalkable(nodes, x + 1, y + 1);
            bool leftDown = IsNeighbourWalkable(nodes, x - 1, y - 1);
            bool rightDown = IsNeighbourWalkable(nodes, x + 1, y - 1);

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

        private bool IsWalkable(MapObject obj)
        {
            MapObjectType type = obj.type;

            return type == MapObjectType.Road || type == MapObjectType.Destination || type == MapObjectType.SpawnPoint;
        }

        private bool IsNeighbourWalkable(NativeArray<Node> nodes, int x, int y)
        {
            if (!IsValidPos(new int2(x, y))) return false;

            return nodes[GetIndex(x, y)].isWalkable;
        }

        private int GetIndex(int x, int y)
        {
            return x * height + y;
        }

        private int GetDistance(int2 a, int2 b)
        {
            int h = math.abs(a.x - b.x);
            int v = math.abs(a.y - b.y);
            int remain = math.abs(h - v);
            return 14 * math.min(h, v) + 10 * remain;
        }

        private bool IsValidPos(int2 pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        private void GetPath(Node endNode, ref NativeList<int2> path)
        {
            Node temp = endNode;
            while (temp.parentIndex != -1)
            {
                path.Add(new int2(temp.x, temp.y));
                temp = nodes[temp.parentIndex];
            }

            path.Add(new int2(temp.x, temp.y));
        }

        private void ResetNodes()
        {
            for (int j = 0; j < nodes.Length; j++)
            {
                Node node = nodes[j];
                node.costG = int.MaxValue;
                node.costH = GetDistance(new int2(node.x, node.y), endPos);
                node.parentIndex = -1;
                nodes[j] = node;
            }
        }
        #endregion

        #region 面向对象
        public void CFindPath(Vector2Int startPos)
        {
            CNode start = cnodes[CGetIndex(startPos.x, startPos.y)];
            start.costG = 0;
            cnodes[start.index] = start;
            int endIndex = CGetIndex(endPos.x, endPos.y);

            List<int> openList = new List<int>();
            HashSet<int> closeList = new HashSet<int>();

            openList.Add(start.index);
            while (openList.Count > 0)
            {
                int currIndex = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (cnodes[openList[i]].costF < cnodes[currIndex].costF)
                    {
                        currIndex = openList[i];
                    }
                }

                // 抵达终点
                if (currIndex == endIndex) break;

                CNode currNode = cnodes[currIndex];

                // 把当前结点从open list移除
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i] == currIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closeList.Add(currIndex);

                //for (int i = 0; i < neighbours.Length; i++)
                var neighbours = CGetNeighbours(currNode);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    CNode neighbourNode = neighbours[i];
                    if (closeList.Contains(neighbourNode.index)) continue; // 已经加入关闭列表

                    int g = currNode.costG + CGetDistance(new Vector2Int(currNode.x, currNode.y), new Vector2Int(neighbourNode.x, neighbourNode.y));

                    if (g < neighbourNode.costG)
                    {
                        neighbourNode.parentIndex = currIndex;
                        neighbourNode.costG = g;
                        cnodes[neighbourNode.index] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            CResetNodes();
        }

        private List<CNode> CGetNeighbours(CNode node)
        {
            List<CNode> neighbours = new List<CNode>();

            int x = node.x;
            int y = node.y;

            bool left = CIsNeighbourWalkable(x - 1, y);
            bool right = CIsNeighbourWalkable(x + 1, y);
            bool up = CIsNeighbourWalkable(x, y + 1);
            bool down = CIsNeighbourWalkable(x, y - 1);

            bool leftUp = CIsNeighbourWalkable(x - 1, y + 1);
            bool rightUp = CIsNeighbourWalkable(x + 1, y + 1);
            bool leftDown = CIsNeighbourWalkable(x - 1, y - 1);
            bool rightDown = CIsNeighbourWalkable(x + 1, y - 1);

            if (left) neighbours.Add(cnodes[CGetIndex(x - 1, y)]);
            if (right) neighbours.Add(cnodes[CGetIndex(x + 1, y)]);
            if (up) neighbours.Add(cnodes[CGetIndex(x, y + 1)]);
            if (down) neighbours.Add(cnodes[CGetIndex(x, y - 1)]);

            if (left && up && leftUp) neighbours.Add(cnodes[CGetIndex(x - 1, y + 1)]);
            if (right && up && rightUp) neighbours.Add(cnodes[CGetIndex(x + 1, y + 1)]);
            if (left && down && leftDown) neighbours.Add(cnodes[CGetIndex(x - 1, y - 1)]);
            if (right && down && rightDown) neighbours.Add(cnodes[CGetIndex(x + 1, y - 1)]);

            return neighbours;
        }

        private bool CIsWalkable(MapObject obj)
        {
            MapObjectType type = obj.type;

            return type == MapObjectType.Road || type == MapObjectType.Destination || type == MapObjectType.SpawnPoint;
        }

        private bool CIsNeighbourWalkable(int x, int y)
        {
            if (!CIsValidPos(new Vector2Int(x, y))) return false;

            return cnodes[CGetIndex(x, y)].isWalkable;
        }

        private int CGetIndex(int x, int y)
        {
            return x * height + y;
        }

        private int CGetDistance(Vector2Int a, Vector2Int b)
        {
            int h = Mathf.Abs(a.x - b.x);
            int v = Mathf.Abs(a.y - b.y);
            int remain = Mathf.Abs(h - v);
            return 14 * Mathf.Min(h, v) + 10 * remain;
        }

        private bool CIsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        private void CResetNodes()
        {
            for (int j = 0; j < cnodes.Count; j++)
            {
                CNode node = cnodes[j];
                node.costG = int.MaxValue;
                node.costH = CGetDistance(new Vector2Int(node.x, node.y), new Vector2Int(endPos.x, endPos.y));
                node.parentIndex = -1;
                cnodes[j] = node;
            }
        }
        #endregion
    }
}
