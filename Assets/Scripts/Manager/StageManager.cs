using System;
using UnityEngine;

namespace TowerDefense
{
    public class StageManager : MonoBehaviour
    {
        public enum State
        {
            Preparing,
            Playing,
            Paused,
            GameOver,
        }


        private LevelData levelData;
        private int hp; // 当前生命
        private int coins; // 当前金币
        private State state = State.Preparing; // 当前游戏的状态

        public InputManager InputManager { get; private set; }

        public MapManager MapManager { get; private set; }

        public EnemyManager EnemyManager { get; private set; }

        public TowerManager TowerManager { get; private set; }

        public bool IsPreparing => state == State.Preparing;
        public bool IsPlaying => state == State.Playing;
        public bool IsPaused => state == State.Paused;
        public bool IsGameOver => state == State.GameOver;

        private void Awake()
        {
            // 设置关卡数据
            levelData = LevelData.CreateDefaultData();
            hp = levelData.playerHp;
            coins = levelData.coins;

            InputManager = new InputManager(this);
            MapManager = new MapManager(this);
            EnemyManager = new EnemyManager(this, levelData.waveInterval, levelData.waveData);
            TowerManager = new TowerManager(this);
        }

        private void Start()
        {
            // 生成地图
            MapManager.CreateMap(levelData.mapWidth, levelData.mapHeight, levelData.mapData, Utils.MAP_CELL_SIZE);
        }

        private void Update()
        {
            InputManager.OnUpdate();

            if (IsPlaying)
            {
                EnemyManager.OnUpdate();
                Physics.SyncTransforms(); // 敌人移动后把Transform信息同步到物理引擎
                TowerManager.OnUpdate();
            }
        }

        public void StartGame()
        {
            if (state != State.Preparing)
            {
                Debug.LogError("当前不处于准备状态" + state);
                return;
            }

            Debug.Log("开始游戏");
            state = State.Playing;
        }

        public void Pause()
        {
            if (state != State.Playing)
            {
                Debug.LogError("当前不处于游戏状态" + state);
                return;
            }

            Debug.Log("暂停");
            state = State.Paused;
        }

        public void Continue()
        {
            if (state != State.Paused)
            {
                Debug.LogError("当前不处于暂停状态" + state);
                return;
            }

            Debug.Log("继续");
            state = State.Playing;
        }

        public void Replay()
        {
            Debug.Log("重玩");
            state = State.Preparing;
            hp = levelData.playerHp;
            coins = levelData.coins;

            EnemyManager.Replay();
            TowerManager.Replay();
        }
    }
}
