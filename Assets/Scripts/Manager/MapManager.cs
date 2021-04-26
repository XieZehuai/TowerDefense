using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TowerDefense
{
    public class MapManager : SubStageManager
    {
        /// <summary>
        /// 地图宽度
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 地图高度
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// 地图上所有出生点
        /// </summary>
        public HashSet<MapObject> SpawnPoints { get; private set; }

        private int cellSize; // 单元格大小
        private PoolObject[,] models; // 地图格子模型
        private MapObject destination; // 目标点

        // 记录摆放格子命令，实现撤销功能
        private readonly Stack<System.Action> commandBuffer = new Stack<System.Action>();

        public MapManager(StageManager stageManager) : base(stageManager)
        {
            SpawnPoints = new HashSet<MapObject>();
        }

        #region 创建以及加载地图

        // 创建默认地图数据
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

        // 加载地图模型以及设置出生点和目标点
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
                        PlaceSpawnPoint(x, y, false);
                    }
                }
            }
        }

        /// <summary>
        /// 保存地图数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void SaveMapData(string fileName)
        {
            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, fileName);

            Map.Save(path);
        }

        /// <summary>
        /// 读取地图数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        // 读取地图数据
        public void LoadMapData(string fileName)
        {
            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, fileName);

            if (File.Exists(path))
            {
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
            else
            {
                Debug.LogWarning("没有地图数据" + fileName);
                CreateMap(20, 20, Utils.MAP_CELL_SIZE);
                SaveMapData(fileName);
            }
        }

        #endregion

        #region 摆放炮塔相关功能

        /// <summary>
        /// 尝试在目标位置摆放炮塔
        /// </summary>
        /// <param name="worldPosition">要摆放的目标位置</param>
        /// <param name="x">成功时返回目标位置的X轴坐标</param>
        /// <param name="y">成功时返回目标位置的Y轴坐标</param>
        /// <param name="towerPosition">成功时保存返回炮塔的实际坐标</param>
        /// <returns>成功或失败</returns>
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

        /// <summary>
        /// 判断是否可以在目标位置摆放炮塔
        /// </summary>
        /// <param name="worldPosition">要摆放的目标位置</param>
        /// <param name="x">成功时返回X轴坐标</param>
        /// <param name="y">成功时返回Y轴坐标</param>
        /// <returns>成功或失败</returns>
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

        /// <summary>
        /// 移除炮塔
        /// </summary>
        /// <param name="x">炮塔的X轴坐标</param>
        /// <param name="y">炮塔的Y轴坐标</param>
        /// <returns>移除成功或失败</returns>
        public bool RemoveTower(int x, int y)
        {
            if (Map.GetGridType(x, y) != MapObjectType.WallWithTower) return false;

            Map.SetGridType(x, y, MapObjectType.Wall);
            return true;
        }

        /// <summary>
        /// 移除所有炮塔
        /// </summary>
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

        public void ChangeGridType(Vector3 worldPosition, MapObjectType type, bool record)
        {
            if (Map.GetGridPosition(worldPosition, out int x, out int y))
            {
                ChangeGridType(x, y, type, record);
            }
        }

        /// <summary>
        /// 改变目标格子的类型
        /// </summary>
        /// <param name="x">格子的X轴坐标</param>
        /// <param name="y">格子的Y轴坐标</param>
        /// <param name="type">目标类型</param>
        /// <param name="record">是否记录本次操作，记录后可以撤销该操作</param>
        public void ChangeGridType(int x, int y, MapObjectType type, bool record = false)
        {
            // 如果要替换的格子类型与当前格子类型相同或者当前格子是终点，直接返回
            if (Map.GetGridType(x, y) == type || Map.GetGridType(x, y) == MapObjectType.WallWithTower)
                return;
            if (Map.GetValue(x, y) == destination)
                return;

            switch (type)
            {
                case MapObjectType.Empty:
                    PlaceEmptyGrid(x, y, record);
                    break;
                case MapObjectType.Road:
                    PlaceRoadGrid(x, y, record);
                    break;
                case MapObjectType.Wall:
                    PlaceWallGrid(x, y, record);
                    break;
                case MapObjectType.SpawnPoint:
                    PlaceSpawnPoint(x, y, record);
                    break;
                case MapObjectType.Destination:
                    PlaceDestination(x, y, record);
                    break;

                default:
                    Debug.LogError("无法摆放" + x + " " + y + " " + type);
                    break;
            }
        }

        /// <summary>
        /// 撤销最后一次修改地图的操作
        /// </summary>
        public void Undo()
        {
            if (commandBuffer.Count > 0)
            {
                commandBuffer.Pop().Invoke();
            }
        }

        private void PlaceEmptyGrid(int x, int y, bool record)
        {
            MapObjectType originType = Map.GetGridType(x, y);

            if (originType == MapObjectType.SpawnPoint || originType == MapObjectType.Road)
            {
                if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

                Map.SetGridType(x, y, MapObjectType.Empty);
                if (!FindPaths(destination))
                {
                    Map.SetGridType(x, y, originType);
                    return;
                }
            }

            UnloadModel(x, y);
            Map.SetGridType(x, y, MapObjectType.Empty);
            LoadModel(x, y, MapObjectType.Empty);

            // 记录当前操作的反向操作
            if (record) commandBuffer.Push(() => ChangeGridType(x, y, originType));
        }

        private void PlaceRoadGrid(int x, int y, bool record)
        {
            MapObjectType originType = Map.GetGridType(x, y);

            if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

            UnloadModel(x, y);
            Map.SetGridType(x, y, MapObjectType.Road);
            LoadModel(x, y, MapObjectType.Road);

            FindPaths(destination);

            // 记录当前操作的反向操作
            if (record) commandBuffer.Push(() => ChangeGridType(x, y, originType));
        }

        private void PlaceWallGrid(int x, int y, bool record)
        {
            MapObjectType originType = Map.GetGridType(x, y);

            if (originType == MapObjectType.SpawnPoint || originType == MapObjectType.Road)
            {
                if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

                Map.SetGridType(x, y, MapObjectType.Wall);
                if (!FindPaths(destination))
                {
                    Map.SetGridType(x, y, originType);
                    return;
                }
            }

            UnloadModel(x, y);
            Map.SetGridType(x, y, MapObjectType.Wall);
            LoadModel(x, y, MapObjectType.Wall);

            // 记录当前操作的反向操作
            if (record) commandBuffer.Push(() => ChangeGridType(x, y, originType));
        }

        private void PlaceDestination(int x, int y, bool record)
        {
            MapObjectType originType = Map.GetGridType(x, y);
            if (originType == MapObjectType.SpawnPoint && !RemoveSpawnPoint(x, y)) return;

            Map.SetGridType(x, y, MapObjectType.Destination);

            if (FindPaths(Map.GetValue(x, y)))
            {
                UnloadModel(x, y);
                int xTemp = destination.x;
                int yTemp = destination.y;
                destination = Map.GetValue(x, y);

                UnloadModel(xTemp, yTemp);
                Map.SetGridType(xTemp, yTemp, MapObjectType.Road);
                LoadModel(xTemp, yTemp, MapObjectType.Road);
                LoadModel(x, y, MapObjectType.Destination);

                // 记录当前操作的反向操作
                if (record) commandBuffer.Push(() => ChangeGridType(x, y, originType));
            }
            else
            {
                Map.SetGridType(x, y, originType);
            }
        }

        private void PlaceSpawnPoint(int x, int y, bool record)
        {
            MapObjectType originType = Map.GetGridType(x, y);
            MapObject obj = Map.GetValue(x, y);
            Vector2Int endPos = new Vector2Int(destination.x, destination.y);

            SpawnPoints.Add(obj);
            if (manager.SetPaths(endPos, false))
            {
                UnloadModel(x, y);
                Map.SetGridType(x, y, MapObjectType.SpawnPoint);
                LoadModel(x, y, MapObjectType.SpawnPoint);

                // 记录当前操作的反向操作
                if (record) commandBuffer.Push(() => ChangeGridType(x, y, originType));
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
            manager.SetPaths(new Vector2Int(destination.x, destination.y), false);

            return true;
        }

        private void LoadModel(int x, int y, MapObjectType type)
        {
            Vector3 pos = Map.GetWorldPosition(x, y);
            PoolObject obj = ObjectPool.Spawn<PoolObject>(type.ToString() + "Prefab", pos, Quaternion.identity, Vector3.one * cellSize);
            models[x, y] = obj;
        }

        private void UnloadModel(int x, int y)
        {
            ObjectPool.Unspawn(models[x, y]);
        }

        #endregion

        private bool FindPaths(MapObject target)
        {
            return manager.SetPaths(new Vector2Int(target.x, target.y), true);
        }
    }
}