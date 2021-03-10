using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class EnemyManager : SubStageManager
    {
        // 关卡敌人数据
        private float waveInterval;
        private float spawnInterval;
        private Dictionary<int, int>[] waveData;

        private float timer = 0f;
        private int currentWave;
        private int enemyCounter;
        private bool spawn;
        private bool nextWave;
        private Dictionary<int, int>.Enumerator enumerator;

        private List<Vector3>[] paths;
        private List<Enemy> enemys = new List<Enemy>();

        public EnemyManager(StageManager stageManager) : base(stageManager)
        {
            TypeEventSystem.Register<OnChangePaths>(OnChangePaths);
        }

        public void SetLevelData(float waveInterval, float spawnInterval, Dictionary<int, int>[] waveData)
        {
            this.waveInterval = waveInterval;
            this.spawnInterval = spawnInterval;
            this.waveData = waveData;

            nextWave = true;
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

            //if (currentWave < waveData.Length)
            //{
            //    timer += Time.deltaTime;

            //    if (timer >= interval)
            //    {
            //        timer = 0f;
            //        interval = spawnInterval;

            //        if (enemyCounter < enumerator.Current.Value)
            //        {
            //            enemyCounter++;
            //            CreateEnemy(enumerator.Current.Key);
            //        }
            //        else if (enumerator.MoveNext())
            //        {
            //            enemyCounter = 1;
            //            CreateEnemy(enumerator.Current.Key);
            //        }
            //        else
            //        {
            //            currentWave++;
            //            if (currentWave < waveData.Length)
            //            {
            //                enumerator = waveData[currentWave].GetEnumerator();
            //                interval = waveInterval;
            //            }
            //        }
            //    }
            //}
        }

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
                CreateEnemy(enumerator.Current.Key);
            }
            else
            {
                spawn = false;
                enemyCounter = 0;
            }
        }

        private void NextWave()
        {
            nextWave = false;
            currentWave++;
            if (currentWave >= waveData.Length) return;

            enumerator = waveData[currentWave].GetEnumerator();
            spawn = true;
        }

        private void CreateEnemy(int id)
        {
            EnemyData enemyData = ConfigManager.Instance.EnemyConfig.GetEnemyData(id); // 随机获取敌人数据
            GameObject enemyObj = ObjectPool.Instance.Spawn(enemyData.name); // 生成对象
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            int random = Random.Range(0, paths.Length); // 随机设置路径
            enemy.SetData(enemyData).SetPath(paths[random]);
            enemys.Add(enemy);
        }

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
                    ObjectPool.Instance.Unspawn(enemys[i].Name, enemys[i].gameObject);
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
