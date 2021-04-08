using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 管理所有敌人
    /// </summary>
    public class EnemyManager : SubStageManager
    {
        // 关卡敌人数据
        private readonly float waveInterval; // 每一波的间隔
        private readonly WaveConfig[] waveDatas; // 每一波的数据

        private float spawnTimer = 0f; // 敌人生成计时器
        private float spawnInterval; // 下一个敌人的生成间隔
        private int currentWaveIndex; // 当前是第几波敌人
        private int currentEnemySequenceCount; // 当前生成到第几个敌人
        private bool isSpawning; // 是否正在生成敌人
        private bool isNextWave; // 是否开始生成下一波敌人
        private IEnumerator<EnemySequence> enumerator; // 当前这波敌人的枚举器
        private List<Vector3>[] spawnPointPaths; // 从生成点到终点的所有路径

        public List<Enemy> Enemys { get; } = new List<Enemy>(); // 保存所有敌人的引用

        public EnemyManager(StageManager stageManager, float waveInterval, WaveConfig[] waveDatas) : base(stageManager)
        {
            this.waveInterval = waveInterval;
            this.waveDatas = waveDatas;

            Replay(); // Replay方法重置场景物体并重新开始游戏，也可用于第一次开始游戏

            TypeEventSystem.Register<NextWave>(SpawnNextWave);
        }

        /// <summary>
        /// 设置所有出生点到终点的路径
        /// </summary>
        /// <param name="paths">路径</param>
        public void SetSpawnPointPaths(List<Vector2Int>[] paths)
        {
            spawnPointPaths = new List<Vector3>[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                spawnPointPaths[i] = new List<Vector3>();

                for (int j = 0; j < paths[i].Count; j++)
                {
                    spawnPointPaths[i].Add(manager.MapManager.GetCenterPosition(paths[i][j]));
                }
            }
        }

        /// <summary>
        /// 设置所有敌人到目标点的路径
        /// </summary>
        /// <param name="paths">路径</param>
        public void SetEnemyPaths(List<Vector2Int>[] paths)
        {
            if (paths.Length != Enemys.Count)
            {
                Debug.LogError("敌人数量与路径数量不相等");
                return;
            }

            for (int i = 0; i < paths.Length; i++)
            {
                List<Vector3> path = new List<Vector3>();

                for (int j = 0; j < paths[i].Count; j++)
                {
                    path.Add(manager.MapManager.GetCenterPosition(paths[i][j]));
                }

                Enemys[i].SetPath(path, false);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (isSpawning || isNextWave)
            {
                spawnTimer += deltaTime;

                if (isSpawning && spawnTimer >= spawnInterval) // 生成敌人
                {
                    spawnTimer = 0f;
                    SpawnEnemy();
                }
                else if (isNextWave)
                {
                    if (currentWaveIndex == -1 || spawnTimer >= waveInterval) // 生成下一波敌人
                    {
                        SpawnNextWave();
                    }
                    else
                    {
                        // 更新下一波敌人的生成倒计时，用于在UI上显示
                        TypeEventSystem.Send(new UpdateNextWaveCountdown { countdown = waveInterval - spawnTimer });
                    }
                }
            }

            UpdateEnemys(deltaTime);
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void Replay()
        {
            if (Enemys.Count > 0)
            {
                for (int i = 0; i < Enemys.Count; i++)
                {
                    ObjectPool.Unspawn(Enemys[i]);
                }

                Enemys.Clear();
            }

            isNextWave = true;
            isSpawning = false;
            spawnTimer = 0f;
            currentWaveIndex = -1;
            spawnInterval = 0f;
            currentEnemySequenceCount = 0;
        }

        // 生成敌人
        private void SpawnEnemy()
        {
            // 当前敌人序列不为空并且没有生成完，就继续生成
            if (enumerator.Current != null && currentEnemySequenceCount < enumerator.Current.count)
            {
                currentEnemySequenceCount++;
                CreateEnemy((int)enumerator.Current.enemyType);
            }
            // 当前序列生成完后，判断是否还有下一个序列，有就切换到下一个序列继续生成
            else if (enumerator.MoveNext())
            {
                currentEnemySequenceCount = 1;
                spawnInterval = enumerator.Current.interval;
                CreateEnemy((int)enumerator.Current.enemyType);
            }
            // 当前这一波敌人的所有序列已经生成完（不能直接生成下一波敌人，要等到所有敌人都被消灭或者抵达终点后再生成下一波）
            else
            {
                isSpawning = false;
                currentEnemySequenceCount = 0;
                spawnInterval = 0f;
            }
        }

        // 生成下一波
        private void SpawnNextWave(NextWave context = default)
        {
            if (!isNextWave) return;

            spawnTimer = 0f;
            isNextWave = false;
            currentWaveIndex++;
            if (currentWaveIndex >= waveDatas.Length) return; // 没有下一波，直接返回

            enumerator = waveDatas[currentWaveIndex].GetEnumerator();
            isSpawning = true;
            TypeEventSystem.Send(new UpdateWaveCount { waveCount = currentWaveIndex + 1 });
        }

        // 生成敌人
        private void CreateEnemy(int id)
        {
            EnemyData enemyData = GameManager.Instance.EnemyConfig.GetEnemyData(id); // 获取敌人数据
            Enemy enemy = ObjectPool.Spawn<Enemy>(enemyData.name);

            enemy.Data = enemyData;
            int random = Random.Range(0, spawnPointPaths.Length); // 随机设置路径
            enemy.SetPath(spawnPointPaths[random], true);

            Enemys.Add(enemy);
        }

        // 执行所有敌人的更新逻辑
        private void UpdateEnemys(float deltaTime)
        {
            // 场景中敌人数量为空，判断是否需要生成下一波敌人并返回
            if (Enemys.Count == 0)
            {
                isNextWave = !isSpawning && currentWaveIndex + 1 < waveDatas.Length;

                if (!isSpawning && currentWaveIndex + 1 >= waveDatas.Length)
                {
                    manager.GameOver();
                }

                return;
            }

            for (int i = 0; i < Enemys.Count; i++)
            {
                if (!Enemys[i].OnUpdate(deltaTime))
                {
                    ObjectPool.Unspawn(Enemys[i]);
                    Enemys.QuickRemove(i--);
                }
            }
        }

        protected override void OnDispose()
        {
            TypeEventSystem.UnRegister<NextWave>(SpawnNextWave);
        }
    }
}