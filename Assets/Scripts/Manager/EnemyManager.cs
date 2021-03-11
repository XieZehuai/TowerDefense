using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class EnemyManager : SubStageManager
    {
        // 关卡敌人数据
        private readonly float waveInterval; // 每一波的间隔
        private readonly Dictionary<int, int>[] waveData; // 每一波及每波包含敌人的数据

        private float timer = 0f; // 敌人生成计时器
        private float spawnInterval; // 下一个敌人的生成间隔
        private int currentWave; // 当前是第几波敌人
        private int enemyCounter; // 当前生成到第几个敌人
        private bool spawn; // 是否正在生成敌人
        private bool nextWave; // 是否开始生成下一波敌人
        private Dictionary<int, int>.Enumerator enumerator; // 当前波敌人的枚举器

        private List<Vector3>[] paths; // 从生成点到终点的所有路径
        private readonly List<Enemy> enemys = new List<Enemy>(); // 保存所有敌人的引用

        public EnemyManager(StageManager stageManager, float waveInterval, Dictionary<int, int>[] waveData) : base(stageManager)
        {
            this.waveInterval = waveInterval;
            this.waveData = waveData;

            Replay();

            TypeEventSystem.Register<OnChangePaths>(OnChangePaths);
        }

        public override void OnUpdate()
        {
            if (spawn || nextWave)
            {
                timer += Time.deltaTime;

                if (spawn && timer >= spawnInterval)
                {
                    timer = 0f;
                    Spawn();
                }
                else if (nextWave && timer >= waveInterval)
                {
                    timer = 0f;
                    NextWave();
                }
            }

            UpdateEnemys();
        }

        public void Replay()
        {
            if (enemys.Count > 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    //ObjectPool.Instance.Unspawn(enemys[i].Name, enemys[i].gameObject);
                    ObjectPool<Enemy>.Unspawn(enemys[i].Name, enemys[i]);
                }

                enemys.Clear();
            }

            nextWave = true;
            spawn = false;
            timer = 0f;
            currentWave = -1;
            spawnInterval = 0f;
            enemyCounter = 0;
        }

        // 生成敌人
        private void Spawn()
        {
            if (enemyCounter < enumerator.Current.Value)
            {
                enemyCounter++;
                CreateEnemy(enumerator.Current.Key);
            }
            else if (enumerator.MoveNext())
            {
                enemyCounter = 1;
                int id = enumerator.Current.Key;
                spawnInterval = ConfigManager.Instance.EnemyConfig.GetEnemyData(id).spawnInterval;
                CreateEnemy(id);
            }
            else
            {
                spawn = false;
                enemyCounter = 0;
            }
        }

        // 生成下一波
        private void NextWave()
        {
            nextWave = false;
            currentWave++;
            if (currentWave >= waveData.Length) return;

            enumerator = waveData[currentWave].GetEnumerator();
            spawn = true;
        }

        // 加载敌人
        private void CreateEnemy(int id)
        {
            EnemyData enemyData = ConfigManager.Instance.EnemyConfig.GetEnemyData(id); // 随机获取敌人数据

            //GameObject enemyObj = ObjectPool.Instance.Spawn(enemyData.name); // 生成对象
            //Enemy enemy = enemyObj.GetComponent<Enemy>();
            Enemy enemy = ObjectPool<Enemy>.Spawn(enemyData.name);

            int random = Random.Range(0, paths.Length); // 随机设置路径
            enemy.SetData(enemyData).SetPath(paths[random]);
            enemys.Add(enemy);
        }

        // 执行所有敌人的更新逻辑
        private void UpdateEnemys()
        {
            if (enemys.Count == 0)
            {
                nextWave = !spawn && currentWave < waveData.Length;
                return;
            }

            for (int i = 0; i < enemys.Count; i++)
            {
                if (!enemys[i].OnUpdate())
                {
                    //ObjectPool.Instance.Unspawn(enemys[i].Name, enemys[i].gameObject);
                    ObjectPool<Enemy>.Unspawn(enemys[i].Name, enemys[i]);
                    enemys.QuickRemove(i--);
                }
            }
        }

        private void OnChangePaths(OnChangePaths context)
        {
            paths = context.paths;
        }
    }
}
