using System;
using UnityEngine;

namespace TowerDefense
{
    public class StageManager : MonoBehaviour
    {
        private LevelData levelData;

        public InputManager InputManager { get; private set; }

        public MapManager MapManager { get; private set; }

        public EnemyManager EnemyManager { get; private set; }

        public TowerManager TowerManager { get; private set; }

        private void Awake()
        {
            levelData = LevelData.CreateDefaultData(); // 生成关卡默认数据

            InputManager = new InputManager(this);
            MapManager = new MapManager(this);
            EnemyManager = new EnemyManager(this, levelData.waveInterval, levelData.waveData);
            TowerManager = new TowerManager(this);
        }

        private void Start()
        {

            //EnemyManager.SetLevelData(levelData.waveInterval, levelData.spawnInterval, levelData.waveData);
            //MapManager.CreateMap(mapSize.x, mapSize.y, cellSize);
            MapManager.CreateMap(levelData.mapWidth, levelData.mapHeight, levelData.mapData, 1);
        }

        private void Update()
        {
            InputManager.OnUpdate();

            EnemyManager.OnUpdate();

            Physics.SyncTransforms(); // 敌人移动后把Transform信息同步到物理引擎

            TowerManager.OnUpdate();
        }

        public void Replay()
        {
            EnemyManager.Replay();
        }
    }
}
