using System;
using UnityEngine;
namespace TowerDefense
{
    /*
     * 减速炮塔，每隔一段时间，减速范围内的敌人
     */
    public class DecelerationTower : Tower
    {
        [SerializeField] private Transform shootPoint = default;
        private Vector3 shootPos;

        private float attackTimer;

        protected override void OnInit()
        {
            shootPos = shootPoint.position;
            attackTimer = Data.levelData.attackDuration;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (attackTimer < Data.levelData.attackDuration)
            {
                attackTimer += deltaTime;
            }
            else if (attackTimer >= Data.levelData.attackDuration)
            {
                if (FindTarget(out Enemy _))
                {
                    DecelerateEnemys();
                    attackTimer = 0f;
                }
            }
        }

        private void DecelerateEnemys()
        {
            Enemy.AttackAll(transform.localPosition, Data.levelData.attackRange, enemy =>
            {
                enemy.Decelerate(Data.levelData.decelerationDuration, Data.levelData.decelerationRate);
            });

            Particle particle = ObjectPool.Spawn<Particle>("DecelerateWave_1_Effect", transform.localPosition);
            particle.SetFloat("CircleSize", Data.levelData.attackRange);
            particle.Play();
            particle.DelayUnspawn(0.5f);
        }
    }
}
