using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 管理关卡场景所有战斗相关的对象
    /// </summary>
    public class WarEntityManager : SubStageManager
    {
        private readonly List<WarEntity> entities = new List<WarEntity>();

        public WarEntityManager(StageManager stageManager) : base(stageManager)
        {
        }

        /// <summary>
        /// 发射炮弹
        /// </summary>
        /// <param name="blastRadius">炮弹爆炸时的攻击范围</param>
        /// <param name="damage">炮弹造成的伤害</param>
        /// <param name="launchPos">炮弹的发射位置</param>
        /// <param name="targetPos">炮弹攻击的目标位置</param>
        /// <param name="velocity">炮弹的初始发射速度</param>
        public void LaunchShell(float blastRadius, float damage, Vector3 launchPos, Vector3 targetPos, Vector3 velocity)
        {
            Shell shell = ObjectPool.Spawn<Shell>(Res.ShellPrefab, launchPos);
            shell.Init(blastRadius, damage, launchPos, targetPos, velocity);
            entities.Add(shell);
        }

        public override void OnUpdate(float deltaTime)
        {
            UpdateWarEntities(deltaTime);
        }

        public void Replay()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                ObjectPool.Unspawn(entities[i]);
            }

            entities.Clear();
        }

        private void UpdateWarEntities(float deltaTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (!entities[i].OnUpdate(deltaTime))
                {
                    ObjectPool.Unspawn(entities[i]);
                    entities.QuickRemove(i--);
                }
            }
        }
    }
}