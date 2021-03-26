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

        private float attackTimer;

        public override AttackType AttackType => AttackType.Special;

        public override void OnUpdate()
        {
            if (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
            }
            else if (attackTimer >= attackDuration)
            {
                attackTimer = 0f;
                DecelerateEnemys();
            }
        }

        private void DecelerateEnemys()
        {
            Enemy.AttackAll(transform.localPosition, attackRange, enemy =>
            {
                enemy.Decelerate(decelerateTime, decelerateRate);
            });
        }
    }
}
