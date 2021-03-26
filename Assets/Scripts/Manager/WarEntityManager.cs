using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class WarEntityManager : SubStageManager
    {
        private readonly List<WarEntity> entities = new List<WarEntity>();

        public WarEntityManager(StageManager stageManager) : base(stageManager)
        {
        }

        public void LaunchShell(float blastRadius, float damage, Vector3 attackPos, Vector3 targetPos, Vector3 velocity)
        {
            Shell shell = ObjectPool.Spawn<Shell>("Shell", attackPos);
            shell.Init(blastRadius, damage, attackPos, targetPos, velocity);
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