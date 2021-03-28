using System;
using UnityEngine;
namespace TowerDefense
{
    /*
     * 减速炮塔，每隔一段时间，减速范围内的敌人
     */
    public class DecelerationTower : Tower
    {
        [SerializeField] private float attackDuration = 2f; // 攻击间隔
        [SerializeField] private float decelerateTime = 3f; // 减速时常
        [SerializeField] private float decelerateRate = 0.8f; // 减速率

        [SerializeField] private Transform shootPoint = default;
        private Vector3 shootPos;

        private float attackTimer;

        public override AttackType AttackType => AttackType.Special;

        public override void OnSpawn()
        {
            shootPos = shootPoint.position;
            attackTimer = attackDuration;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (attackTimer < attackDuration)
            {
                attackTimer += deltaTime;
            }
            else if (attackTimer >= attackDuration)
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
            Enemy.AttackAll(transform.localPosition, attackRange, enemy =>
            {
                enemy.Decelerate(decelerateTime, decelerateRate);
            });

            Particle particle = ObjectPool.Spawn<Particle>("DecelerateWave_1_Effect", transform.localPosition);
            particle.SetFloat("CircleSize", attackRange);
            particle.Play();
            particle.DelayUnspawn(0.5f);
        }
    }
}
