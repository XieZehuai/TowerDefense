using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 游戏关卡管理器，控制整个关卡的流程
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        // 当前游戏的状态
        public enum GameState
        {
            Preparing,
            Playing,
            Paused,
            GameOver,
        }

        private int stage; // 当前关卡数
        private StageConfig stageConfig; // 当前关卡配置
        private int hp; // 当前生命
        private int coins; // 当前金币
        private GameState state = GameState.Preparing; // 当前游戏的状态
        private GameState stateTemp; // 在暂停时保存原状态，继续游戏后恢复原状态
        private bool isSpeedUp; // 当前是否处于加速模式
        private PathFinder pathFinder;

        #region 实现各种功能的子管理器
        public InputManager InputManager { get; private set; }
        public MapManager MapManager { get; private set; }
        public EnemyManager EnemyManager { get; private set; }
        public TowerManager TowerManager { get; private set; }
        public WarEntityManager WarEntityManager { get; private set; }
        public PathIndicator PathIndicator { get; private set; }
        [SerializeField] private CameraController cameraController = default;
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
            stageConfig = GameManager.Instance.GetStageConfig(stage); // 获取关卡数据
            hp = stageConfig.playerHp;
            coins = stageConfig.coins;

            // 初始化子管理器
            InputManager = new InputManager(this);
            MapManager = new MapManager(this);
            EnemyManager = new EnemyManager(this, stageConfig.waveInterval, stageConfig.waveDatas);
            TowerManager = new TowerManager(this);
            WarEntityManager = new WarEntityManager(this);
            PathIndicator = new PathIndicator(this);

            // 设置寻路策略
            PathFindingStrategy strategy = GameManager.Instance.pathFindingStrategy;
            IPathFindingStrategy pathFindingStrategy = GetPathFindingStrategy(strategy);
            pathFinder = new PathFinder(pathFindingStrategy);

            TypeEventSystem.Register<OnEnemyReach>(OnEnemyReach);
            TypeEventSystem.Register<OnEnemyDestroy>(OnEnemyDestroy);
        }

        private IPathFindingStrategy GetPathFindingStrategy(PathFindingStrategy strategy)
        {
            switch (strategy)
            {
                case PathFindingStrategy.Dijkstra: return new DijkstraPathFinding();
                case PathFindingStrategy.ReverseDijkstra: return new ReverseDijkstraPathFinding();
                case PathFindingStrategy.DOTS: return new DOTSPathFinding();

                default: Debug.LogError("不支持的寻路策略" + strategy); break;
            }

            return null;
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

            // 打开游戏UI
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
                Physics.SyncTransforms(); // 敌人移动后把Transform信息同步到物理引擎，避免炮塔检测敌人出错
                TowerManager.OnUpdate(deltaTime);
                WarEntityManager.OnUpdate(deltaTime);
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (state != GameState.Preparing)
            {
                Debug.LogError("当前不处于准备状态" + state);
                return;
            }

            state = GameState.Playing;
        }

        /// <summary>
        /// 暂停
        /// </summary>
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

        /// <summary>
        /// 继续
        /// </summary>
        public void Continue()
        {
            if (state != GameState.Paused)
            {
                Debug.LogError("当前不处于暂停状态" + state);
                return;
            }

            state = stateTemp; // 恢复原状态
        }

        /// <summary>
        /// 重新开始
        /// </summary>
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

        /// <summary>
        /// 进入或退出加速模式
        /// </summary>
        /// <param name="speedUp">是否加速</param>
        public void SpeedUp(bool speedUp)
        {
            isSpeedUp = speedUp;
        }

        /// <summary>
        /// 保存地图数据
        /// </summary>
        public void SaveMapData()
        {
            Debug.Log("保存地图数据");
            MapManager.SaveMapData(Utils.MAP_DATA_FILENAME_PREFIX + stage);
        }

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public void LoadMapData()
        {
            Debug.Log("加载地图数据");
            MapManager.LoadMapData(Utils.MAP_DATA_FILENAME_PREFIX + stage);
        }

        /// <summary>
        /// 设置出生点以及敌人的行动路径
        /// </summary>
        /// <param name="endPos">目标点</param>
        /// <param name="includeEnemy">是否包含敌人</param>
        /// <returns>设置成功或失败</returns>
        public bool SetPaths(Vector2Int endPos, bool includeEnemy)
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

            // 寻路，并在成功时设置所有出生点以及敌人的移动路径
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

        /// <summary>
        /// 自定义起始点的寻路，寻路成功时把路径保存到传入的数组里
        /// </summary>
        /// <param name="startPosArray">所有起点组成的数组</param>
        /// <param name="endPos">目标点</param>
        /// <param name="getPath">寻路成功时是否保存得到的路径</param>
        /// <param name="paths">用于存储所有路径的容器</param>
        /// <returns>成功或失败</returns>
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