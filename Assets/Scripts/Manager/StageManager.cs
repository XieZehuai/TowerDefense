using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private CameraController cameraController = default;

        private LevelData levelData;
        private int hp; // 当前生命
        private int coins; // 当前金币
        private State state = State.Preparing; // 当前游戏的状态
        private State stateTemp; // 在暂停时保存原状态，继续游戏后恢复原状态

        public InputManager InputManager { get; private set; }
        public MapManager MapManager { get; private set; }
        public EnemyManager EnemyManager { get; private set; }
        public TowerManager TowerManager { get; private set; }
        public PathIndicator PathIndicator { get; private set; }
        public PathFinder PathFinder { get; private set; }
        public CameraController CameraController => cameraController;

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
            PathIndicator = new PathIndicator(this);

            if (GameManager.Instance.pathFindingStrategy == PathFindingStrategy.Dijkstra)
                PathFinder = new PathFinder(new DijkstraPathFinding());
            else if (GameManager.Instance.pathFindingStrategy == PathFindingStrategy.ReverseDijkstra)
                PathFinder = new PathFinder(new ReverseDijkstraPathFinding());
            else if (GameManager.Instance.pathFindingStrategy == PathFindingStrategy.DOTS)
                PathFinder = new PathFinder(new DOTSPathFinding());

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
                MapManager.CreateMap(levelData.mapWidth, levelData.mapHeight, levelData.mapData, Utils.MAP_CELL_SIZE);
            }

            UIManager.Instance.Open<UIGameScene>(new UIGameSceneData { maxHp = hp, coins = coins, maxWaveCount = levelData.waveData.Length }, UILayer.Background);
        }

        private void Update()
        {
            InputManager.OnUpdate();

            if (IsPlaying)
            {
                TowerManager.OnUpdate();
                EnemyManager.OnUpdate();
            }
        }

        public void StartGame()
        {
            if (state != State.Preparing)
            {
                Debug.LogError("当前不处于准备状态" + state);
                return;
            }

            state = State.Playing;
        }

        public void Pause()
        {
            if (state == State.Paused)
            {
                Debug.LogError("已经处于暂停状态");
                return;
            }

            stateTemp = state; // 保存原状态
            state = State.Paused;
        }

        public void Continue()
        {
            if (state != State.Paused)
            {
                Debug.LogError("当前不处于暂停状态" + state);
                return;
            }

            state = stateTemp; // 恢复原状态
        }

        public void Replay()
        {
            state = State.Preparing;
            hp = levelData.playerHp;
            coins = levelData.coins;

            EnemyManager.Replay();
            TowerManager.Replay();

            TypeEventSystem.Send(new OnReplay());
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
            int enemyCount = includeEnemy ? EnemyManager.enemys.Count : 0;
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
                    if (MapManager.GetGridPosition(EnemyManager.enemys[i - spawnPointCount].NextWayPoint, out int x, out int y))
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
            PathFinder.SetMapData(MapManager.Map.GridArray);

            if (PathFinder.FindPaths(startPosArray, endPos, ref paths, true))
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
            PathFinder.SetMapData(MapManager.Map.GridArray);

            return PathFinder.FindPaths(startPosArray, endPos, ref paths, true);
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

        private void OnDestroy()
        {
            TypeEventSystem.UnRegister<OnEnemyReach>(OnEnemyReach);
            TypeEventSystem.UnRegister<OnEnemyDestroy>(OnEnemyDestroy);

            InputManager.Dispose();
            MapManager.Dispose();
            EnemyManager.Dispose();
            TowerManager.Dispose();
            PathIndicator.Dispose();
        }
    }
}
