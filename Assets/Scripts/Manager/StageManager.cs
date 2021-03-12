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
        private State stateTemp; // 在暂停时保存原状态，继续游戏后恢复原状态

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

            if (hp <= 0) Debug.LogError("玩家初始生命值必须大于0" + hp);

            InputManager = new InputManager(this);
            MapManager = new MapManager(this);
            EnemyManager = new EnemyManager(this, levelData.waveInterval, levelData.waveData);
            TowerManager = new TowerManager(this);

            TypeEventSystem.Register<OnEnemyReach>(OnEnemyReach);
            TypeEventSystem.Register<OnEnemyDestroy>(OnEnemyDestroy);
            TypeEventSystem.Register<StartGame>(StartGame);
            TypeEventSystem.Register<PauseGame>(Pause);
            TypeEventSystem.Register<ContinueGame>(Continue);
            TypeEventSystem.Register<ReplayGame>(Replay);
        }

        private void Start()
        {
            // 生成地图
            MapManager.CreateMap(levelData.mapWidth, levelData.mapHeight, levelData.mapData, Utils.MAP_CELL_SIZE);

            UIManager.Instance.Open<UIGameScene>(new UIGameSceneData { maxHp = hp, coins = coins, maxWaveCount = levelData.waveData.Length }, UILayer.Background);
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

        private void OnEnemyReach(OnEnemyReach context)
        {
            hp--;
            TypeEventSystem.Send(new UpdateHp { hp = hp });

            if (hp <= 0)
            {
                Debug.Log("游戏结束");
                state = State.GameOver;
            }
        }

        private void OnEnemyDestroy(OnEnemyDestroy context)
        {
            coins += context.reward;
            TypeEventSystem.Send(new UpdateCoins { coins = coins });
        }

        public void StartGame(StartGame context = default)
        {
            if (state != State.Preparing)
            {
                Debug.LogError("当前不处于准备状态" + state);
                return;
            }

            Debug.Log("开始游戏");
            state = State.Playing;
        }

        public void Pause(PauseGame context = default)
        {
            if (state != State.Playing && state != State.Preparing)
            {
                Debug.LogError("当前不处于游戏状态" + state);
                return;
            }

            Debug.Log("暂停");
            stateTemp = state; // 保存原状态
            state = State.Paused;
        }

        public void Continue(ContinueGame context = default)
        {
            if (state != State.Paused)
            {
                Debug.LogError("当前不处于暂停状态" + state);
                return;
            }

            Debug.Log("继续");
            state = stateTemp; // 恢复原状态
        }

        public void Replay(ReplayGame context = default)
        {
            Debug.Log("重玩");
            state = State.Preparing;
            hp = levelData.playerHp;
            coins = levelData.coins;

            EnemyManager.Replay();
            TowerManager.Replay();

            TypeEventSystem.Send(new OnReplay());
        }

        private void OnDestroy()
        {
            TypeEventSystem.UnRegister<OnEnemyReach>(OnEnemyReach);
            TypeEventSystem.UnRegister<OnEnemyDestroy>(OnEnemyDestroy);
            TypeEventSystem.UnRegister<StartGame>(StartGame);
            TypeEventSystem.UnRegister<PauseGame>(Pause);
            TypeEventSystem.UnRegister<ContinueGame>(Continue);
            TypeEventSystem.UnRegister<ReplayGame>(Replay);

            InputManager.Dispose();
            MapManager.Dispose();
            EnemyManager.Dispose();
            TowerManager.Dispose();
        }
    }
}
