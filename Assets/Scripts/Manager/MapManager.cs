using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TowerDefense
{
    public class MapManager : SubStageManager
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Map Map { get; private set; }
        public HashSet<MapObject> SpawnPoints { get; private set; }
        //public Dictionary<MapObject, List<Vector3>> paths;

        private int cellSize;
        private PoolObject[,] models;
        private MapObject destination;

        public MapManager(StageManager stageManager) : base(stageManager)
        {
            SpawnPoints = new HashSet<MapObject>();
            //paths = new Dictionary<MapObject, List<Vector3>>();
        }

        #region 创建以及加载地图
        public void CreateMap(int width, int height, int cellSize)
        {
            this.Width = width;
            this.Height = height;
            this.cellSize = cellSize;

            models = new PoolObject[width, height];
            Map = new Map(width, height, cellSize, new Vector3(-width / 2f, 0f, -height / 2f) * cellSize);
            LoadMap();
        }

        public void CreateMap(int width, int height, MapObjectType[] datas, int cellSize)
        {
            CreateMap(ToMapObjectData(width, height, datas), cellSize);
        }

        public void CreateMap(MapObject[,] data, int cellSize)
        {
            Width = data.GetLength(0);
            Height = data.GetLength(1);
            this.cellSize = cellSize;

            models = new PoolObject[Width, Height];
            Map = new Map(data, cellSize, new Vector3(-Width / 2f, 0f, -Height / 2f) * cellSize);
            LoadMap();
        }

        private MapObject[,] ToMapObjectData(int width, int height, MapObjectType[] datas)
        {
            MapObject[,] mapData = new MapObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mapData[x, y] = new MapObject(datas[x * height + y], x, y);
                }
            }

            return mapData;
        }

        private void LoadMap()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    MapObjectType type = Map.GetGridType(x, y);
                    LoadModel(x, y, type);

                    if (type == MapObjectType.Destination)
                    {
                        if (destination == null)
                            destination = Map.GetValue(x, y);
                        else
                            Debug.LogError("有多个目标点");
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Map.GetGridType(x, y) == MapObjectType.SpawnPoint)
                    {
                        PlaceSpawnPoint(x, y);
                    }
                }
            }
        }

        public void SaveMapData(string fileName)
        {
            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, fileName);

            Map.Save(path);
        }

        public void LoadMapData(string fileName)
        {
            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, fileName);

            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                int cellSize = reader.ReadInt32();
                MapObject[,] grids = new MapObject[width, height];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        MapObjectType type = (MapObjectType)reader.ReadInt32();
                        grids[x, y] = new MapObject(type, x, y);
                    }
                }

                CreateMap(grids, cellSize);
            }
        }
        #endregion

        #region 摆放炮塔相关功能
        public bool TryPlaceTower(Vector3 worldPosition, out int x, out int y, out Vector3 towerPosition)
        {
            if (CanPlaceTower(worldPosition, out x, out y))
            {
                Map.SetGridType(x, y, MapObjectType.WallWithTower);
                towerPosition = GetCenterPosition(x, y) + new Vector3(0f, 0.25f, 0f);
                return true;
            }

            towerPosition = Vector3.zero;
            return false;
        }

        public bool CanPlaceTower(Vector3 worldPosition, out int x, out int y)
        {
            if (GetGridPosition(worldPosition, out x, out y))
            {
                if (Map.GetGridType(x, y) == MapObjectType.Wall)
                {
                    return true;
                }
            }

            x = 0;
            y = 0;
            return false;
        }

        public bool RemoveTower(int x, int y)
        {
            if (Map.GetGridType(x, y) != MapObjectType.WallWithTower) return false;

            Map.SetGridType(x, y, MapObjectType.Wall);
            return true;
        }

        public void RemoveAllTower()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Map.GetGridType(x, y) == MapObjectType.WallWithTower)
                    {
                        Map.SetGridType(x, y, MapObjectType.Wall);
                    }
                }
            }
        }
        #endregion

        #region 格子相关功能
        public bool GetGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            return Map.GetGridPosition(worldPosition, out x, out y);
        }

        public Vector3 GetCenterPosition(Vector2Int pos)
        {
            return GetCenterPosition(pos.x, pos.y);
        }

        public Vector3 GetCenterPosition(int x, int y)
        {
            return Map.GetCenterPosition(x, y);
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return Map.GetWorldPosition(x, y);
        }

        public void ChangeGridType(Vector3 worldPosition, MapObjectType type)
        {
            if (Map.GetGridPosition(worldPosition, out int x, out int y))
            {
                ChangeGridType(x, y, type);
            }
        }

        public void ChangeGridType(int x, int y, MapObjectType type)
        {
            // 如果要替换的格子类型与当前格子类型相同或者当前格子是终点，直接返回
            if (Map.GetGridType(x, y) == type || Map.GetGridType(x, y) == MapObjectType.WallWithTower)
                return;
            if (Map.GetValue(x, y) == destination)
                return;

            switch (type)
            {
                case MapObjectType.Empty:
                    PlaceEmptyGrid(x, y);
                    break;
                case MapObjectType.Road:
                    PlaceRoadGrid(x, y);
                    break;
                case MapObjectType.Wall:
                    PlaceWallGrid(x, y);
                    break;
                case MapObjectType.SpawnPoint:
                    PlaceSpawnPoint(x, y);
                    break;
                case MapObjectType.Destination:
                    PlaceDestination(x, y);
                    break;

                default:
                    Debug.LogError("无法摆放" + x + " " + y + " " + type);
                    break;
            }
        }

        private void PlaceEmptyGrid(int x, int y)
        {
            MapObjectType originType = Map.GetGridType(x, y);

            if (originType == MapObjectType.SpawnPoint || originType == MapObjectType.Road)
            {
                if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

                Map.SetGridType(x, y, MapObjectType.Empty);
                if (!FindAllPath(destination))
                {
                    Map.SetGridType(x, y, originType);
                    return;
                }
            }

            UnloadModel(x, y, originType);
            Map.SetGridType(x, y, MapObjectType.Empty);
            LoadModel(x, y, MapObjectType.Empty);
        }

        private void PlaceRoadGrid(int x, int y)
        {
            MapObjectType originType = Map.GetGridType(x, y);

            if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

            UnloadModel(x, y);
            Map.SetGridType(x, y, MapObjectType.Road);
            LoadModel(x, y, MapObjectType.Road);

            FindAllPath(destination);
        }

        private void PlaceWallGrid(int x, int y)
        {
            MapObjectType originType = Map.GetGridType(x, y);

            if (originType == MapObjectType.SpawnPoint || originType == MapObjectType.Road)
            {
                if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

                Map.SetGridType(x, y, MapObjectType.Wall);
                if (!FindAllPath(destination))
                {
                    Map.SetGridType(x, y, originType);
                    return;
                }
            }

            UnloadModel(x, y, originType);
            Map.SetGridType(x, y, MapObjectType.Wall);
            LoadModel(x, y, MapObjectType.Wall);
        }

        private void PlaceDestination(int x, int y)
        {
            MapObjectType originType = Map.GetGridType(x, y);
            if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

            Map.SetGridType(x, y, MapObjectType.Destination);

            if (FindAllPath(Map.GetValue(x, y)))
            {
                UnloadModel(x, y, originType);
                int xTemp = destination.x;
                int yTemp = destination.y;
                destination = Map.GetValue(x, y);

                UnloadModel(xTemp, yTemp);
                Map.SetGridType(xTemp, yTemp, MapObjectType.Road);
                LoadModel(xTemp, yTemp, MapObjectType.Road);

                LoadModel(x, y, MapObjectType.Destination);
            }
            else
            {
                Map.SetGridType(x, y, originType);
            }
        }

        private void PlaceSpawnPoint(int x, int y)
        {
            //List<Vector3> path = new List<Vector3>();
            MapObject obj = Map.GetValue(x, y);
            Vector2Int endPos = new Vector2Int(destination.x, destination.y);

            SpawnPoints.Add(obj);
            //if (Dijkstra(obj, destination, true, path))
            if (manager.FindPaths(endPos, false))
            {
                UnloadModel(x, y);
                Map.SetGridType(x, y, MapObjectType.SpawnPoint);
                LoadModel(x, y, MapObjectType.SpawnPoint);
                //this.paths.Add(obj, path);
                //TypeEventSystem.Send(new OnChangePaths { paths = GetPaths() });
            }
            else
            {
                SpawnPoints.Remove(obj);
            }
        }

        private bool RemoveSpawnPoint(int x, int y)
        {
            if (SpawnPoints.Count <= 1) return false;

            MapObject obj = Map.GetValue(x, y);
            if (obj == null) return false;

            SpawnPoints.Remove(obj);
            //paths.Remove(obj);
            manager.FindPaths(new Vector2Int(destination.x, destination.y), false);
            //TypeEventSystem.Send(new OnChangePaths { paths = GetPaths() });

            return true;
        }

        private void LoadModel(int x, int y, MapObjectType type)
        {
            Vector3 pos = Map.GetWorldPosition(x, y);
            PoolObject obj = ObjectPool.Spawn<PoolObject>(type.ToString(), pos, Quaternion.identity, Vector3.one * cellSize);
            //Transform obj = GenericObjectPool<Transform>.Spawn(type.ToString(), pos, Quaternion.identity, Vector3.one * cellSize);
            models[x, y] = obj;
        }

        private void UnloadModel(int x, int y)
        {
            UnloadModel(x, y, Map.GetGridType(x, y));
        }

        private void UnloadModel(int x, int y, MapObjectType type)
        {
            //GenericObjectPool<Transform>.Unspawn(type.ToString(), models[x, y]);
            ObjectPool.Unspawn(type.ToString(), models[x, y]);
        }
        #endregion

        #region 路径相关功能
        //public List<Vector3>[] GetPaths()
        //{
        //    return paths.Values.ToArray();
        //}

        private bool FindAllPath(MapObject target)
        {
            return manager.FindPaths(new Vector2Int(target.x, target.y), true);

            //if (spawnPoints.Count == 0) return false;

            //Dictionary<MapObject, List<Vector3>> tempPaths = new Dictionary<MapObject, List<Vector3>>();

            //foreach (var point in spawnPoints)
            //{
            //    List<Vector3> path = new List<Vector3>();
            //    if (!Dijkstra(point, target, true, path))
            //    //if (!AStar(point, target, true, path))
            //    {
            //        return false;
            //    }

            //    tempPaths.Add(point, path);
            //}

            //paths = tempPaths;
            //TypeEventSystem.Send(new OnChangePaths { paths = GetPaths() });
            //return true;
        }
        #endregion

        #region 寻路算法
        // A*寻路算法
        private bool AStar(MapObject start, MapObject end, bool getPath, List<Vector3> path = null)
        {
            List<MapObject> openList = new List<MapObject>();
            HashSet<MapObject> closeList = new HashSet<MapObject>();
            MapObject[,] parents = new MapObject[Width, Height];

            int[,] costG = new int[Width, Height]; // 保存起点到每个点的距离
            int[,] costH = new int[Width, Height]; // 保存每个点到终点的距离

            int CostF(MapObject obj) => costH[obj.x, obj.y] + costG[obj.x, obj.y];
            int CostG(MapObject obj) => costG[obj.x, obj.y];

            openList.Add(start);
            while (openList.Count > 0)
            {
                // 找出最优节点，每次都从最优节点开始查找路径，最优节点为总距离及到终点距离最小的节点
                MapObject curr = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //if (CostF(openList[i]) < CostF(curr) && CostH(openList[i]) < CostH(curr))
                    if (CostF(openList[i]) < CostF(curr))
                    {
                        curr = openList[i];
                    }
                }
                // 找到最优节点后，移动到closeList然后开始寻路
                openList.Remove(curr);
                closeList.Add(curr);

                // 如果已经找到了终点，就获取路径并返回true
                if (curr == end)
                {
                    if (getPath && path != null)
                    {
                        GetPathWithPos(parents, start, end, path);
                    }

                    return true;
                }

                // 遍历当前节点所有相邻并且可行走的节点
                foreach (MapObject node in GetAroundGrid(curr))
                {
                    // 跳过已经搜索过的节点
                    if (closeList.Contains(node)) continue;

                    int g = CostG(curr) + GetDistance(curr, node); // 计算从起点到curr再到node的距离
                    bool hasSearch = openList.Contains(node); // 判断node有没有被搜索过

                    // 如果从起点到curr再到node的距离小于从起点到node的距离，
                    // 或者node没有被搜索过，就更新node的路径，指向curr
                    if (g <= CostG(node) || !hasSearch)
                    {
                        costG[node.x, node.y] = g;
                        costH[node.x, node.y] = GetDistance(node, end);
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

        // Dijkstra寻路算法
        private bool Dijkstra(MapObject start, MapObject end, bool getPath, List<Vector3> path = null)
        {
            PriorityQueue<MapObject> openList = new PriorityQueue<MapObject>(8); // 保存所有待寻路的节点（可用优先队列保存）
            HashSet<MapObject> closeList = new HashSet<MapObject>(); // 保存所有已经寻路过的节点
            MapObject[,] parents = new MapObject[Width, Height]; // 每个节点的父节点，构成一棵树，形成路径

            openList.Add(start);
            while (!openList.IsEmpty)
            {
                MapObject curr = openList.DeleteMax();
                closeList.Add(curr);

                // 如果已经找到了终点，就获取路径并返回true
                if (curr == end)
                {
                    if (getPath && path != null)
                    {
                        GetPathWithPos(parents, start, end, path);
                    }

                    return true;
                }

                // 遍历当前节点所有相邻并且可行走的节点
                foreach (MapObject node in GetAroundGrid(curr))
                {
                    // 跳过已经搜索过的节点
                    if (closeList.Contains(node)) continue;

                    int g = curr.costG + GetDistance(curr, node);
                    bool hasSearch = openList.Contains(node); // 判断node有没有被搜索过

                    // 如果从起点到curr再到node的距离小于从起点到node的距离，
                    // 或者node没有被搜索过，就更新node的路径，指向curr
                    if (g <= node.costG || !hasSearch)
                    {
                        node.costG = g;
                        parents[node.x, node.y] = curr;

                        // 没搜索过就加入openList，等待搜索
                        if (!hasSearch)
                        {
                            node.costH = GetDistance(node, end);
                            openList.Add(node);
                        }
                    }
                }
            }

            return false;
        }

        // 判断目标格子是否可行走
        private bool IsWalkable(MapObject obj)
        {
            return obj != null && obj.IsWalkable();
        }

        // 使用估算法计算两个点之间的距离（对角线以及直线距离相加）
        private int GetDistance(MapObject from, MapObject to)
        {
            int x = Mathf.Abs(from.x - to.x);
            int y = Mathf.Abs(from.y - to.y);

            return x > y ? (14 * y + 10 * (x - y)) : (14 * x + 10 * (y - x));
        }

        // 获取所有与当前格子相邻并且可行走的格子
        private IEnumerable<MapObject> GetAroundGrid(MapObject obj)
        {
            List<MapObject> list = new List<MapObject>();
            int x = obj.x;
            int y = obj.y;

            MapObject left = Map.GetValue(x - 1, y);
            MapObject right = Map.GetValue(x + 1, y);
            MapObject up = Map.GetValue(x, y + 1);
            MapObject down = Map.GetValue(x, y - 1);

            MapObject leftUp = Map.GetValue(x - 1, y + 1);
            MapObject rightUp = Map.GetValue(x + 1, y + 1);
            MapObject leftDown = Map.GetValue(x - 1, y - 1);
            MapObject rightDown = Map.GetValue(x + 1, y - 1);

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
        private void GetPathWithPos(MapObject[,] parents, MapObject start, MapObject end, List<Vector3> path)
        {
            MapObject temp = end;

            while (temp != start)
            {
                path.Add(Map.GetCenterPosition(temp.x, temp.y));
                temp = parents[temp.x, temp.y];
            }

            path.Add(Map.GetCenterPosition(start.x, start.y));

            path.Reverse();
        }
        #endregion
    }
}
