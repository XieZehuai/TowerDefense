using System;
using UnityEngine;

namespace TowerDefense
{
    public class StageManager : MonoBehaviour
    {
        private LevelData levelData;
        private MapObjectType selectedType = MapObjectType.Empty;

        public MapManager MapManager { get; private set; }

        public EnemyManager EnemyManager { get; private set; }

        public TowerManager TowerManager { get; private set; }

        private void Awake()
        {
            MapManager = new MapManager(this);
            EnemyManager = new EnemyManager(this);
            TowerManager = new TowerManager(this);
        }

        private void Start()
        {
            levelData = LevelData.CreateDefaultData();

            EnemyManager.SetLevelData(levelData.waveInterval, levelData.spawnInterval, levelData.waveData);
            //MapManager.CreateMap(mapSize.x, mapSize.y, cellSize);
            MapManager.CreateMap(levelData.mapWidth, levelData.mapHeight, levelData.mapData, 1);
        }

        private void Update()
        {
            ChangeMap();

            EnemyManager.OnUpdate();

            Physics.SyncTransforms();

            TowerManager.OnUpdate();
        }

        private void ChangeMap()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedType = MapObjectType.Empty;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedType = MapObjectType.Road;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedType = MapObjectType.Wall;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedType = MapObjectType.SpawnPoint;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectedType = MapObjectType.Destination;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Utils.GetMousePosition();
                MapManager.ChangeGridType(pos, selectedType);
            }

            if (Input.GetMouseButtonDown(1))
            {
                TowerManager.CreateTower(Utils.GetMousePosition());
            }

            if (Input.GetMouseButtonDown(2))
            {
                TowerManager.RemoveTower(Utils.GetMousePosition());
            }
        }
    }
}
