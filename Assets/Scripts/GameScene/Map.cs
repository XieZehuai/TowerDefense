using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    #region 地图格子的类型
    /// <summary>
    /// 地图格子的类型
    /// </summary>
    public enum MapObjectType
    {
        Empty, // 空格子
        Road, // 敌人行走的道路
        Wall, // 防止炮塔的墙壁
        SpawnPoint, // 敌人的出生点
        Destination, // 敌人的目标点

        None, // 非格子的其他地方
    }
    #endregion


    #region 地图上的格子
    /// <summary>
    /// 地图上的格子
    /// </summary>
    public class MapObject : IComparable<MapObject>
    {
        public MapObjectType type;
        public int x;
        public int y;

        public int CostG { get; set; } // 从起点到当前节点的距离

        public int CostH { get; set; } // 从当前节点到终点的距离

        public int CostF => CostG + CostH; // 从起点经过当前节点到终点的距离

        public MapObject()
        {
            type = MapObjectType.Road;
            x = 0;
            y = 0;
        }

        public MapObject(MapObjectType type, int x, int y)
        {
            this.type = type;
            this.x = x;
            this.y = y;
        }

        public bool IsWalkable()
        {
            return type != MapObjectType.Wall && type != MapObjectType.Empty && type != MapObjectType.None;
        }

        public override int GetHashCode()
        {
            return x << 16 + y;
        }

        public int CompareTo(MapObject other)
        {
            return other.CostG - CostG;
            //if (CostF < other.CostF) return 1;
            //if (CostF > other.CostF && CostH > other.CostH) return -1;
            //return 0;
        }
    }
    #endregion


    public class Map : Grid<MapObject>
    {
        private GameObject[,] objs;
        private Transform gridParent;
        private HashSet<MapObject> spawnPoints;
        private MapObject destination;

        public Map(int width, int height, int cellSize, Vector3 originPos) : base(width, height, cellSize, originPos)
        {
            objs = new GameObject[width, height];
            spawnPoints = new HashSet<MapObject>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridArray[x, y] = new MapObject(MapObjectType.Road, x, y);
                }
            }

            gridArray[0, 0].type = MapObjectType.SpawnPoint;
            spawnPoints.Add(gridArray[0, 0]);

            gridArray[width - 1, height - 1].type = MapObjectType.Destination;
            destination = gridArray[width - 1, height - 1];
        }

        public Map(int width, int height, int cellSize) : this(width, height, cellSize, Vector3.zero)
        {
        }

        public Map(MapObject[,] objects, int cellSize, Vector3 originPos) : base(objects, cellSize, originPos)
        {
            objs = new GameObject[width, height];
            spawnPoints = new HashSet<MapObject>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (GetGridType(x, y) == MapObjectType.SpawnPoint)
                    {
                        spawnPoints.Add(GetValue(x, y));
                    }
                    else if (GetGridType(x, y) == MapObjectType.Destination)
                    {
                        destination = GetValue(x, y);
                    }
                }
            }
        }

        public Map(MapObject[,] objects, int cellSize) : this(objects, cellSize, Vector3.zero)
        {
        }

        public void SetGridParent(Transform parent)
        {
            gridParent = parent;
        }

        public void Instantiate()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    LoadModel(x, y, GetGridType(x, y));
                }
            }
        }

        public void SetGridType(int x, int y, MapObjectType type)
        {
            MapObjectType currType = GetGridType(x, y);

            // 如果要替换的格子类型与当前格子类型相同或者当前格子是终点，直接返回
            if (currType == type || destination == GetValue(x, y))
            {
                return;
            }
            else if (currType == MapObjectType.SpawnPoint)
            {
                if (spawnPoints.Count <= 1) return;

                spawnPoints.Remove(gridArray[x, y]);
            }

            UnloadModel(x, y);
            gridArray[x, y].type = type;

            if (type == MapObjectType.SpawnPoint)
            {
                spawnPoints.Add(gridArray[x, y]);
            }
            else if (type == MapObjectType.Destination)
            {
                int xTemp = destination.x;
                int yTemp = destination.y;
                destination = GetValue(x, y);
                SetGridType(xTemp, yTemp, MapObjectType.Road);
            }

            LoadModel(x, y, type);
        }

        public void SetGridType(Vector3 worldPosition, MapObjectType type)
        {
            if (GetGridPosition(worldPosition, out int x, out int y))
            {
                SetGridType(x, y, type);
            }
        }

        public override void SetValue(int x, int y, MapObject value)
        {
            SetGridType(x, y, value.type);
        }

        public override void SetValue(Vector3 worldPosition, MapObject value)
        {
            if (GetGridPosition(worldPosition, out int x, out int y))
            {
                SetGridType(x, y, value.type);
            }
        }

        public MapObjectType GetGridType(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return GetValue(x, y).type;
            }

            return MapObjectType.None;
        }

        public MapObjectType GetGridType(Vector3 worldPosition)
        {
            if (GetGridPosition(worldPosition, out int x, out int y))
            {
                return GetGridType(x, y);
            }

            return MapObjectType.None;
        }

        private void LoadModel(int x, int y, MapObjectType type)
        {
            //string prefabName = type + "Prefab";
            //GameObject obj = ResourceManager.Load<GameObject>(prefabName);
            //Vector3 pos = GetWorldPosition(x, y);
            //obj = GameObject.Instantiate(obj, pos, Quaternion.identity, gridParent);
            //obj.transform.localScale *= cellSize;
            //objs[x, y] = obj;

            Vector3 pos = GetWorldPosition(x, y);
            GameObject obj = ObjectPool.Instance.Spawn(type.ToString(), pos);
            obj.transform.localScale *= cellSize;
            objs[x, y] = obj;
        }

        private void UnloadModel(int x, int y)
        {
            Debug.Log(GetGridType(x, y).ToString());
            ObjectPool.Instance.Unspawn(GetGridType(x, y).ToString(), objs[x, y]);
        }

        public bool SetPath()
        {
            if (spawnPoints.Count == 0) return false;

            foreach (var point in spawnPoints)
            {
                List<MapObject> path = new List<MapObject>();

                if (FindPath(point, destination, ref path))
                {
                    Debug.Log("成功");
                    ShowPath(path);
                }
                else
                {
                    Debug.Log("失败");
                    return false;
                }
            }

            return true;
        }

        private void ShowPath(List<MapObject> path)
        {
            if (path.Count == 0) return;

            foreach (var item in path)
            {
                objs[item.x, item.y].transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }

        #region 深度优先搜索寻路算法
        // 基于深度优先搜索的寻路算法
        //private bool FindPath(MapObject start, List<MapObject> path)
        //{
        //    if (!IsWalkable(start)) return false;

        //    Queue<MapObject> queue = new Queue<MapObject>();
        //    bool[,] hasSearch = new bool[width, height];
        //    MapObject[,] edgeTo = new MapObject[width, height];

        //    queue.Enqueue(start);
        //    hasSearch[start.x, start.y] = true;

        //    while (queue.Count > 0)
        //    {
        //        MapObject temp = queue.Dequeue();
        //        Debug.Log(temp.type);

        //        if (temp.type == MapObjectType.Destination)
        //        {
        //            while (temp != start)
        //            {
        //                path.Add(temp);
        //                temp = edgeTo[temp.x, temp.y];
        //            }

        //            path.Add(start);
        //            return true;
        //        }

        //        int x = temp.x;
        //        int y = temp.y;

        //        bool left = IsWalkable(GetValue(x - 1, y));
        //        bool right = IsWalkable(GetValue(x + 1, y));
        //        bool up = IsWalkable(GetValue(x, y + 1));
        //        bool down = IsWalkable(GetValue(x, y - 1));
        //        bool leftDown = IsWalkable(GetValue(x - 1, y - 1));
        //        bool rightDown = IsWalkable(GetValue(x + 1, y - 1));
        //        bool leftUp = IsWalkable(GetValue(x - 1, y + 1));
        //        bool rightUp = IsWalkable(GetValue(x + 1, y + 1));

        //        if (x - 1 >= 0 && y - 1 >= 0 && !hasSearch[x - 1, y - 1] && left && down && leftDown)
        //        {
        //            queue.Enqueue(GetValue(x - 1, y - 1));
        //            hasSearch[x - 1, y - 1] = true;
        //            edgeTo[x - 1, y - 1] = temp;
        //        }
        //        if (x + 1 < width && y - 1 >= 0 && !hasSearch[x + 1, y - 1] && right && down && rightDown)
        //        {
        //            queue.Enqueue(GetValue(x + 1, y - 1));
        //            hasSearch[x + 1, y - 1] = true;
        //            edgeTo[x + 1, y - 1] = temp;
        //        }
        //        if (x - 1 >= 0 && y + 1 < height && !hasSearch[x - 1, y + 1] && left && up && leftUp)
        //        {
        //            queue.Enqueue(GetValue(x - 1, y + 1));
        //            hasSearch[x - 1, y + 1] = true;
        //            edgeTo[x - 1, y + 1] = temp;
        //        }
        //        if (x + 1 < width && y + 1 < height && !hasSearch[x + 1, y + 1] && right && up && rightUp)
        //        {
        //            queue.Enqueue(GetValue(x + 1, y + 1));
        //            hasSearch[x + 1, y + 1] = true;
        //            edgeTo[x + 1, y + 1] = temp;
        //        }
        //        if (x - 1 >= 0 && !hasSearch[x - 1, y] && left)
        //        {
        //            queue.Enqueue(GetValue(x - 1, y));
        //            hasSearch[x - 1, y] = true;
        //            edgeTo[x - 1, y] = temp;
        //        }
        //        if (x + 1 < width && !hasSearch[x + 1, y] && right)
        //        {
        //            queue.Enqueue(GetValue(x + 1, y));
        //            hasSearch[x + 1, y] = true;
        //            edgeTo[x + 1, y] = temp;
        //        }
        //        if (y - 1 >= 0 && !hasSearch[x, y - 1] && down)
        //        {
        //            queue.Enqueue(GetValue(x, y - 1));
        //            hasSearch[x, y - 1] = true;
        //            edgeTo[x, y - 1] = temp;
        //        }
        //        if (y + 1 < height && !hasSearch[x, y + 1] && up)
        //        {
        //            queue.Enqueue(GetValue(x, y + 1));
        //            hasSearch[x, y + 1] = true;
        //            edgeTo[x, y + 1] = temp;
        //        }
        //    }

        //    Debug.Log("寻路失败");
        //    return false;
        //}
        #endregion

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
        private bool FindPath(MapObject start, MapObject end, ref List<MapObject> path)
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

            MapObject left = GetValue(x - 1, y);
            MapObject right = GetValue(x + 1, y);
            MapObject up = GetValue(x, y + 1);
            MapObject down = GetValue(x, y - 1);

            MapObject leftUp = GetValue(x - 1, y + 1);
            MapObject rightUp = GetValue(x + 1, y + 1);
            MapObject leftDown = GetValue(x - 1, y - 1);
            MapObject rightDown = GetValue(x + 1, y - 1);

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
        private List<MapObject> GetPathWithPos(MapObject[,] parents, MapObject start, MapObject end)
        {
            List<MapObject> list = new List<MapObject>();
            MapObject temp = end;

            while (temp != start)
            {
                list.Add(temp);
                temp = parents[temp.x, temp.y];
            }

            return list;
        }
        #endregion
    }
}
