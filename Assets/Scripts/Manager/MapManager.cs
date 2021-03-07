using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class MapManager : MonoSingleton<MapManager>
    {
        public MapObjectType type = MapObjectType.Empty;

        public HashSet<MapObject> spawnPoints;
        public Dictionary<MapObject, List<Vector3>> paths;

        private int width;
        private int height;
        private int cellSize;
        private GameObject[,] objs;
        private MapObject destination;
        private Map map;

        protected override void OnInit()
        {
            spawnPoints = new HashSet<MapObject>();
            paths = new Dictionary<MapObject, List<Vector3>>();
        }

        public void CreateMap(int width, int height, int cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            objs = new GameObject[width, height];
            map = new Map(width, height, cellSize, new Vector3(-width / 2f, 0f, -height / 2f) * cellSize);
            InitMap();
        }

        public void CreateMap(MapObject[,] data, int cellSize)
        {
            width = data.GetLength(0);
            height = data.GetLength(1);
            this.cellSize = cellSize;

            objs = new GameObject[width, height];
            map = new Map(data, cellSize, new Vector3(-width / 2f, 0f, -height / 2f) * cellSize);
            InitMap();
        }

        public List<Vector3>[] GetPaths()
        {
            return paths.Values.ToArray();
        }

        public void InitMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MapObjectType type = map.GetGridType(x, y);
                    LoadModel(x, y, type);

                    if (type == MapObjectType.Destination)
                    {
                        if (destination == null) destination = map.GetValue(x, y);
                        else Debug.LogError("有多个目标点");
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map.GetGridType(x, y) == MapObjectType.SpawnPoint)
                    {
                        AddSpawnPoint(x, y);
                    }
                }
            }
        }

        public void SetGridType(int x, int y, MapObjectType type)
        {
            MapObjectType currType = map.GetGridType(x, y);

            // 如果要替换的格子类型与当前格子类型相同或者当前格子是终点，直接返回
            if (currType == type || destination == map.GetValue(x, y))
            {
                return;
            }
            else if (currType == MapObjectType.SpawnPoint)
            {
                if (spawnPoints.Count <= 1) return;

                RemoveSpawnPoint(x, y);
            }

            UnloadModel(x, y);
            map.SetGridType(x, y, type);

            if (type == MapObjectType.SpawnPoint)
            {
                AddSpawnPoint(x, y);
            }
            else if (type == MapObjectType.Destination)
            {
                int xTemp = destination.x;
                int yTemp = destination.y;
                destination = map.GetValue(x, y);
                SetGridType(xTemp, yTemp, MapObjectType.Road);
            }

            LoadModel(x, y, type);
        }

        public void SetGridType(Vector3 worldPosition, MapObjectType type)
        {
            if (map.GetGridPosition(worldPosition, out int x, out int y))
            {
                SetGridType(x, y, type);
            }
        }

        private void LoadModel(int x, int y, MapObjectType type)
        {
            Vector3 pos = map.GetWorldPosition(x, y);
            GameObject obj = ObjectPool.Instance.Spawn(type.ToString(), pos);
            obj.transform.localScale = Vector3.one * cellSize;
            objs[x, y] = obj;
        }

        private void UnloadModel(int x, int y)
        {
            ObjectPool.Instance.Unspawn(map.GetGridType(x, y).ToString(), objs[x, y]);
        }

        private void AddSpawnPoint(int x, int y)
        {
            MapObject obj = map.GetValue(x, y);
            if (obj != null)
            {
                spawnPoints.Add(obj);
                List<Vector3> path = new List<Vector3>();
                FindPath(obj, destination, ref path);
                paths.Add(obj, path);
                ShowPath(path);
            }
        }

        private void RemoveSpawnPoint(int x, int y)
        {
            MapObject obj = map.GetValue(x, y);
            if (obj != null)
            {
                spawnPoints.Remove(obj);
                paths.Remove(obj);
            }
        }

        private void ShowPath(List<Vector3> path)
        {
            GameObject prefab = ResourceManager.Load<GameObject>("WayPointPrefab");
            foreach (Vector3 pos in path)
            {
                GameObject.Instantiate(prefab, pos, Quaternion.identity);
            }
        }

        // 判断目标格子是否可行走
        private bool IsWalkable(MapObject obj)
        {
            return obj != null && obj.IsWalkable();
        }

        #region A*寻路算法
        /// <summary>
        /// A*寻路算法，寻路成功返回true并把路径保存到传入的path变量里
        /// 寻路失败返回false，不改变path变量
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="path">用于寻路成功时保存路径</param>
        /// <returns>成功返回true，失败返回false</returns>
        private bool FindPath(MapObject start, MapObject end, ref List<Vector3> path)
        {
            PriorityQueue<MapObject> openList = new PriorityQueue<MapObject>(8); // 保存所有待寻路的节点（可用优先队列保存）
            HashSet<MapObject> closeList = new HashSet<MapObject>(); // 保存所有已经寻路过的节点
            MapObject[,] parents = new MapObject[width, height]; // 每个节点的父节点，构成一棵树，形成路径
            //int[,] costG = new int[width, height]; // 保存起点到每个点的距离
            //int[,] costH = new int[width, height]; // 保存每个点到终点的距离

            //int CostF(MapObject obj) => costH[obj.x, obj.y] + costG[obj.x, obj.y];
            //int CostG(MapObject obj) => costG[obj.x, obj.y];
            //int CostH(MapObject obj) => costH[obj.x, obj.y];

            openList.Add(start);
            while (!openList.IsEmpty)
            {
                // 找出最优节点，每次都从最优节点开始查找路径，最优节点为总距离及到终点距离最小的节点
                //MapObject curr = openList[0];
                //for (int i = 1; i < openList.Count; i++)
                //{
                //    if (CostF(openList[i]) < CostF(curr) && CostH(openList[i]) < CostH(curr))
                //    {
                //        curr = openList[i];
                //    }
                //}
                MapObject curr = openList.DeleteMax();

                // 找到最优节点后，移动到closeList然后开始寻路
                //openList.Remove(curr);
                closeList.Add(curr);

                // 如果已经找到了终点，就获取路径并返回true
                if (curr == end)
                {
                    path = GetPathWithPos(parents, start, end);
                    return true;
                }

                // 遍历当前节点所有相邻并且可行走的节点
                List<MapObject> neighbours = GetAroundGrid(curr);
                foreach (MapObject node in neighbours)
                {
                    // 跳过已经搜索过的节点
                    if (closeList.Contains(node)) continue;

                    //int g = CostG(curr) + GetDistance(curr, node); // 计算从起点到curr再到node的距离
                    int g = curr.CostG + GetDistance(curr, node);
                    bool hasSearch = openList.Contains(node); // 判断node有没有被搜索过

                    // 如果从起点到curr再到node的距离小于从起点到node的距离，
                    // 或者node没有被搜索过，就更新node的路径，指向curr
                    //if (g <= CostG(node) || !hasSearch)
                    if (g <= node.CostG || !hasSearch)
                    {
                        //costG[node.x, node.y] = g;
                        //costH[node.x, node.y] = GetDistance(node, end);
                        node.CostG = g;
                        node.CostH = GetDistance(node, end);
                        parents[node.x, node.y] = curr;

                        // 没搜索过就加入openList，等待搜索
                        if (!hasSearch)
                        {
                            openList.Add(node);
                        }
                    }
                }
            }

            return false;
        }

        // 使用估算法计算两个点之间的距离（对角线以及直线距离相加）
        private int GetDistance(MapObject from, MapObject to)
        {
            int x = Mathf.Abs(from.x - to.x);
            int y = Mathf.Abs(from.y - to.y);

            if (x > y)
            {
                return 14 * y + 10 * (x - y);
            }
            else
            {
                return 14 * x + 10 * (y - x);
            }
        }

        // 获取所有与当前格子相邻并且可行走的格子
        private List<MapObject> GetAroundGrid(MapObject obj)
        {
            List<MapObject> list = new List<MapObject>();
            int x = obj.x;
            int y = obj.y;

            MapObject left = map.GetValue(x - 1, y);
            MapObject right = map.GetValue(x + 1, y);
            MapObject up = map.GetValue(x, y + 1);
            MapObject down = map.GetValue(x, y - 1);

            MapObject leftUp = map.GetValue(x - 1, y + 1);
            MapObject rightUp = map.GetValue(x + 1, y + 1);
            MapObject leftDown = map.GetValue(x - 1, y - 1);
            MapObject rightDown = map.GetValue(x + 1, y - 1);

            if (IsWalkable(left)) list.Add(left);
            if (IsWalkable(right)) list.Add(right);
            if (IsWalkable(up)) list.Add(up);
            if (IsWalkable(down)) list.Add(down);

            if (IsWalkable(leftUp) && IsWalkable(left) && IsWalkable(up)) list.Add(leftUp);
            if (IsWalkable(rightUp) && IsWalkable(right) && IsWalkable(up)) list.Add(rightUp);
            if (IsWalkable(leftDown) && IsWalkable(left) && IsWalkable(down)) list.Add(leftDown);
            if (IsWalkable(rightDown) && IsWalkable(right) && IsWalkable(down)) list.Add(rightDown);

            return list;
        }

        // 获取路径
        private List<Vector3> GetPathWithPos(MapObject[,] parents, MapObject start, MapObject end)
        {
            Stack<Vector3> stack = new Stack<Vector3>();
            MapObject temp = end;

            while (temp != start)
            {
                stack.Push(map.GetCenterPosition(temp.x, temp.y));
                temp = parents[temp.x, temp.y];
            }

            stack.Push(map.GetCenterPosition(start.x, start.y));

            return stack.ToList();
        }
        #endregion
    }
}
