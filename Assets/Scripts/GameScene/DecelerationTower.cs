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

        protected override void OnSetData()
        {
            shootPos = shootPoint.position;
            attackTimer = Data.LevelData.attackDuration;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (attackTimer < Data.LevelData.attackDuration)
            {
                attackTimer += deltaTime;
            }
            else if (attackTimer >= Data.LevelData.attackDuration)
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
            Enemy.AttackAll(transform.localPosition, Data.LevelData.attackRange, enemy =>
            {
                enemy.Decelerate(Data.LevelData.decelerationDuration, Data.LevelData.decelerationRate);
            });

            //Particle particle = ObjectPool.Spawn<Particle>("DecelerateWave_1_Effect", transform.localPosition);
            Particle particle = ObjectPool.Spawn<Particle>(Res.DecelerateWave_1_EffectPrefab, transform.localPosition);
            particle.SetFloat("CircleSize", Data.LevelData.attackRange);
            particle.Play();
            particle.DelayUnspawn(0.5f);
        }
    }
}
