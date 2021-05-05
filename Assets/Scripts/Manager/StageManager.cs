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
		#region 关卡的状态

		/// <summary>
		/// 关卡的状态
		/// </summary>
		private enum State
		{
			Preparing, // 准备阶段，敌人还没开始进攻
			Playing, // 敌人正在进攻
			Paused, // 暂停
			GameOver, // 游戏结束
		}

		#endregion


		private int hp; // 当前生命
		private int coins; // 当前金币

		private State state = State.Preparing; // 当前游戏的状态
		private State stateTemp; // 在暂停时保存原状态，继续游戏后恢复原状态

		private PathFinder pathFinder;

		#region 实现各种功能的子管理器

		public InputManager InputManager { get; private set; }
		public MapManager MapManager { get; private set; }
		public EnemyManager EnemyManager { get; private set; }
		public TowerManager TowerManager { get; private set; }
		public WarEntityManager WarEntityManager { get; private set; }
		public PathIndicator PathIndicator { get; private set; }

		#endregion

		public bool IsPreparing => state == State.Preparing;
		public bool IsPlaying => state == State.Playing;
		public bool IsPaused => state == State.Paused;
		public bool IsGameOver => state == State.GameOver;

		/// <summary>
		/// 当前是否处于加速模式
		/// </summary>
		public bool IsSpeedUp { get; private set; }

		/// <summary>
		/// 当前关卡的配置
		/// </summary>
		public StageConfig StageConfig { get; private set; }

		/// <summary>
		/// 当前玩家的生命值
		/// </summary>
		public int HP
		{
			get => hp;
			set
			{
				hp = value;
				TypeEventSystem.Send(new UpdateHp { hp = hp });
			}
		}

		/// <summary>
		/// 当前玩家的金币
		/// </summary>
		public int Coins
		{
			get => coins;
			set
			{
				coins = value;
				TypeEventSystem.Send(new UpdateCoins { coins = coins });
			}
		}

		private void Awake()
		{
			// 设置关卡数据
			StageConfig = GameManager.Instance.GetStageConfig(PlayerManager.Instance.Data.currentStage); // 获取关卡数据
			hp = StageConfig.hp;
			coins = StageConfig.coins;

			// 初始化子管理器
			InputManager = new InputManager(this);
			MapManager = new MapManager(this);
			EnemyManager = new EnemyManager(this, StageConfig.waveInterval, StageConfig.waveDatas);
			TowerManager = new TowerManager(this);
			WarEntityManager = new WarEntityManager(this);
			PathIndicator = new PathIndicator(this);
			pathFinder = new PathFinder();

			TypeEventSystem.Register<OnEnemyReach>(OnEnemyReach);
			TypeEventSystem.Register<OnEnemyDestroy>(OnEnemyDestroy);
		}

		private void Start()
		{
			LoadMapData();

			// 打开游戏UI
			UIManager.Instance.Open<UIGameScene>(new UIGameSceneData { manager = this });

			// 设置相机的初始位置
			CameraController.Instance.ResetPosition();
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			InputManager.OnUpdate(deltaTime); // 玩家输入不受加速或减速影响

			if (IsSpeedUp) deltaTime *= 2f; // 加速模式游戏速度变为原来的两倍

			if (IsPlaying)
			{
				EnemyManager.OnUpdate(deltaTime);
				Physics.SyncTransforms(); // 敌人移动后把Transform信息同步到物理引擎，避免炮塔检测敌人出错
				TowerManager.OnUpdate(deltaTime);
				WarEntityManager.OnUpdate(deltaTime);

				if (CheckGameOver())
				{
					GameOver();
				}
			}
		}

		/// <summary>
		/// 开始游戏
		/// </summary>
		public void StartGame()
		{
			if (state != State.Preparing)
			{
				Debug.LogError("当前不处于准备状态" + state);
				return;
			}

			state = State.Playing;
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public void Pause()
		{
			if (state == State.Paused)
			{
				Debug.LogError("已经处于暂停状态");
				return;
			}

			stateTemp = state; // 保存原状态
			state = State.Paused;

			UIManager.Instance.Open<UIPausePanel>(new UIPausePanelData { manager = this }, UILayer.Foreground);
		}

		/// <summary>
		/// 继续
		/// </summary>
		public void Continue()
		{
			if (state != State.Paused)
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
			state = State.Preparing;
			HP = StageConfig.hp;
			Coins = StageConfig.coins;
			IsSpeedUp = false;

			EnemyManager.Replay();
			TowerManager.Replay();
			WarEntityManager.Replay();

			TypeEventSystem.Send(new OnReplay());
		}

		/// <summary>
		/// 检查游戏是否结束
		/// </summary>
		private bool CheckGameOver()
		{
			// 游戏结束的条件是消灭了所有敌人或没有生命值
			return EnemyManager.IsAllEnemyDestroy() || hp <= 0;
		}

		/// <summary>
		/// 游戏结束，进行结算
		/// </summary>
		private void GameOver()
		{
			state = State.GameOver;

			if (hp > 0)
			{
				int starCount = StageConfig.GetScore(hp); // 获取关卡评分
				PlayerManager.Instance.StageSuccess(starCount);
				UIManager.Instance.Open<UIStageSuccess>(new UIStageSuccessData
				{
					stage = PlayerManager.Instance.Data.currentStage,
					starCount = starCount
				});
			}
			else
			{
				UIManager.Instance.Open<UIStageFailed>(new UIStageFailedData { onReplayBtnClick = Replay });
			}
		}

		/// <summary>
		/// 进入或退出加速模式
		/// </summary>
		/// <param name="speedUp">是否加速</param>
		public void SpeedUp(bool speedUp)
		{
			IsSpeedUp = speedUp;
		}

		/// <summary>
		/// 保存地图数据
		/// </summary>
		public void SaveMapData()
		{
			Debug.Log("保存地图数据");
			MapManager.SaveMapData(Utils.MAP_DATA_FILENAME_PREFIX + PlayerManager.Instance.Data.currentStage);
		}

		/// <summary>
		/// 加载地图数据
		/// </summary>
		public void LoadMapData()
		{
			string fileName =
				Utils.MAP_DATA_FILENAME_PREFIX + PlayerManager.Instance.Data.currentStage; // 获取当前关卡对应地图数据的文件名
			MapManager.LoadMapData(fileName); // 根据文件名读取地图数据并生成地图
		}

		/// <summary>
		/// 设置出生点以及敌人的行动路径
		/// </summary>
		/// <param name="endPos">目标点</param>
		/// <param name="includeEnemy">是否包含敌人</param>
		/// <returns>设置成功或失败</returns>
		public bool SetPaths(Vector2Int endPos, bool includeEnemy)
		{
			// // 处理起始点数据
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

			if (totalCount >= 20)
			{
				pathFinder.SetPathFindingStrategy(PathFindingStrategy.FlowField);
			}
			else
			{
				pathFinder.SetPathFindingStrategy(PathFindingStrategy.DOTS);
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

		/// <summary>
		/// 敌人抵达终点时由事件系统触发
		/// </summary>
		private void OnEnemyReach(OnEnemyReach context)
		{
			HP--;

			if (HP <= 0)
			{
				GameOver();
			}
		}

		/// <summary>
		/// 敌人被消灭时由事件系统出发
		/// </summary>
		/// <param name="context">事件参数，包含消灭敌人后获得的奖励金币</param>
		private void OnEnemyDestroy(OnEnemyDestroy context)
		{
			Coins += context.reward;
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
