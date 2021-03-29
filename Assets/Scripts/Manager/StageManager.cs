using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    public class StageManager : MonoBehaviour
    {
        public enum GameState
        {
            Preparing,
            Playing,
            Paused,
            GameOver,
        }

        [SerializeField] private CameraController cameraController = default;

        private int stage; // 当前关卡数
        private StageConfig stageConfig; // 当前关卡配置
        private int hp; // 当前生命
        private int coins; // 当前金币
        private GameState state = GameState.Preparing; // 当前游戏的状态
        private GameState stateTemp; // 在暂停时保存原状态，继续游戏后恢复原状态
        private bool isSpeedUp;
        private PathFinder pathFinder;

        #region 实现各种功能的子管理器
        public InputManager InputManager { get; private set; }
        public MapManager MapManager { get; private set; }
        public EnemyManager EnemyManager { get; private set; }
        public TowerManager TowerManager { get; private set; }
        public WarEntityManager WarEntityManager { get; private set; }
        public PathIndicator PathIndicator { get; private set; }
        public CameraController CameraController => cameraController;
        #endregion

        public bool IsPreparing => state == GameState.Preparing;
        public bool IsPlaying => state == GameState.Playing;
        public bool IsPaused => state == GameState.Paused;
        public bool IsGameOver => state == GameState.GameOver;

        private void Awake()
        {
            // 设置关卡数据
            stage = GameManager.Instance.Stage;
            // stageConfig = ConfigManager.Instance.GetStageConfig(stage);
            stageConfig = GameManager.Instance.GetStageConfig(stage); // 获取关卡数据
            hp = stageConfig.playerHp;
            coins = stageConfig.coins;

            if (hp <= 0) Debug.LogError("玩家初始生命值必须大于0" + hp);

            InputManager = new InputManager(this);
            MapManager = new MapManager(this);
            EnemyManager = new EnemyManager(this, stageConfig.waveInterval, stageConfig.waveDatas);
            TowerManager = new TowerManager(this);
            WarEntityManager = new WarEntityManager(this);
            PathIndicator = new PathIndicator(this);

            // 设置寻路策略
            if (GameManager.Instance.pathFindingStrategy == PathFindingStrategy.Dijkstra)
                pathFinder = new PathFinder(new DijkstraPathFinding());
            else if (GameManager.Instance.pathFindingStrategy == PathFindingStrategy.ReverseDijkstra)
                pathFinder = new PathFinder(new ReverseDijkstraPathFinding());
            else if (GameManager.Instance.pathFindingStrategy == PathFindingStrategy.DOTS)
                pathFinder = new PathFinder(new DOTSPathFinding());

            TypeEventSystem.Register<OnEnemyReach>(OnEnemyReach);
            TypeEventSystem.Register<OnEnemyDestroy>(OnEnemyDestroy);
        }

        private void Start()
        {
            // 生成地图
            if (GameManager.Instance.load)
            {
                LoadMapData();
            }
            else
            {
                MapManager.CreateMap(20, 20, Utils.MAP_CELL_SIZE);
            }

            UIManager.Instance.Open<UIGameScene>(
                new UIGameSceneData { maxHp = hp, coins = coins, maxWaveCount = stageConfig.waveDatas.Length },
                UILayer.Background);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (isSpeedUp) deltaTime *= 2f;

            InputManager.OnUpdate(deltaTime);

            if (IsPlaying)
            {
                EnemyManager.OnUpdate(deltaTime);
                Physics.SyncTransforms();
                TowerManager.OnUpdate(deltaTime);
                WarEntityManager.OnUpdate(deltaTime);
            }
        }

        public void StartGame()
        {
            if (state != GameState.Preparing)
            {
                Debug.LogError("当前不处于准备状态" + state);
                return;
            }

            state = GameState.Playing;
        }

        public void Pause()
        {
            if (state == GameState.Paused)
            {
                Debug.LogError("已经处于暂停状态");
                return;
            }

            stateTemp = state; // 保存原状态
            state = GameState.Paused;
        }

        public void Continue()
        {
            if (state != GameState.Paused)
            {
                Debug.LogError("当前不处于暂停状态" + state);
                return;
            }

            state = stateTemp; // 恢复原状态
        }

        public void Replay()
        {
            state = GameState.Preparing;
            hp = stageConfig.playerHp;
            coins = stageConfig.coins;

            EnemyManager.Replay();
            TowerManager.Replay();
            WarEntityManager.Replay();

            TypeEventSystem.Send(new OnReplay());
        }

        public void SpeedUp(bool speedUp)
        {
            isSpeedUp = speedUp;
        }

        public void SaveMapData()
        {
            Debug.Log("保存地图数据");
            MapManager.SaveMapData(GameManager.Instance.fileName);
        }

        public void LoadMapData()
        {
            Debug.Log("加载地图数据");
            MapManager.LoadMapData(GameManager.Instance.fileName);
        }

        public bool FindPaths(Vector2Int endPos, bool includeEnemy)
        {
            // 处理起始点数据
            List<MapObject> spawnPoints = MapManager.SpawnPoints.ToList();

            int spawnPointCount = spawnPoints.Count;
            int enemyCount = includeEnemy ? EnemyManager.Enemys.Count : 0;
            int totalCount = spawnPointCount + enemyCount;

            Vector2Int[] startPosArray = new Vector2Int[totalCount];
            List<Vector2Int>[] paths = new List<Vector2Int>[totalCount];

            for (int i = 0; i < totalCount; i++)
            {
                if (i < spawnPointCount)
                {
                    startPosArray[i] = new Vector2Int(spawnPoints[i].x, spawnPoints[i].y);
                }
                else
                {
                    if (MapManager.GetGridPosition(EnemyManager.Enemys[i - spawnPointCount].NextWayPoint, out int x,
                        out int y))
                    {
                        startPosArray[i] = new Vector2Int(x, y);
                    }
                    else
                    {
                        return false;
                    }
                }

                paths[i] = new List<Vector2Int>();
            }

            // 寻路并设置路径数据
            pathFinder.SetMapData(MapManager.Map.GridArray);

            if (pathFinder.FindPaths(startPosArray, endPos, ref paths, false))
            {
                List<Vector2Int>[] spawnPointPaths = paths.SubArray(0, spawnPointCount);
                EnemyManager.SetSpawnPointPaths(spawnPointPaths);
                PathIndicator.SetPath(spawnPointPaths);

                List<Vector2Int>[] enemyPaths = paths.SubArray(spawnPointCount, enemyCount);
                EnemyManager.SetEnemyPaths(enemyPaths);
                return true;
            }

            return false;
        }

        public bool FindPaths(Vector2Int[] startPosArray, Vector2Int endPos, bool getPath, ref List<Vector2Int>[] paths)
        {
            pathFinder.SetMapData(MapManager.Map.GridArray);

            return pathFinder.FindPaths(startPosArray, endPos, ref paths, false);
        }

        private void OnEnemyReach(OnEnemyReach context)
        {
            hp--;
            TypeEventSystem.Send(new UpdateHp { hp = hp });

            if (hp <= 0)
            {
                Debug.Log("游戏结束");
                state = GameState.GameOver;
            }
        }

        private void OnEnemyDestroy(OnEnemyDestroy context)
        {
            coins += context.reward;
            TypeEventSystem.Send(new UpdateCoins { coins = coins });
        }

        private void OnDestroy()
        {
            TypeEventSystem.UnRegister<OnEnemyReach>(OnEnemyReach);
            TypeEventSystem.UnRegister<OnEnemyDestroy>(OnEnemyDestroy);

            InputManager.Dispose();
            MapManager.Dispose();
            EnemyManager.Dispose();
            TowerManager.Dispose();
            WarEntityManager.Dispose();
            PathIndicator.Dispose();
        }
    }
}