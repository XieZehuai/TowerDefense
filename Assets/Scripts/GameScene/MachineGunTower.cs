using UnityEngine;

namespace TowerDefense
{
    public class MachineGunTower : Tower
    {
        [SerializeField] private float damage = 7f;
        [SerializeField] private float attackDuration = 0.1f; // 攻击间隔

        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform gun = default;
        [SerializeField] private Transform attackPoint = default;

        private Enemy target;
        private float attackTimer;

        public float Damage => damage;

        public float AttackDuration => AttackDuration;

        public override AttackType AttackType => AttackType.MachineGun;

        public override void OnUpdate()
        {
            if (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
            }

            if (TrackTarget(ref target) || FindTarget(out target))
            {
                LookTarget();

                if (attackTimer >= attackDuration)
                {
                    attackTimer = 0f;
                    Attack();
                }
            }
        }

        private void LookTarget()
        {
            Vector3 pos = target.LocalPosition;
            pos.y = 0f;
            turretBase.LookAt(pos);

            pos = (target.LocalPosition - gun.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);
        }

        private void Attack()
        {
            ShowAttackEffect();

            target.GetDamage(damage, AttackType);
        }

        private void ShowAttackEffect()
        {
            ObjectPool.Spawn<PoolObject>("MuzzleEffect", attackPoint.position, attackPoint.rotation, Vector3.one).DelayUnspawn(1.5f);
        }
    }
}
