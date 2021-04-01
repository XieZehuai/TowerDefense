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
        [SerializeField] private LineRenderer lineRenderer = default;

        private Enemy target;
        private float attackTimer;

        public float Damage => damage;

        public float AttackDuration => AttackDuration;

        public override AttackType AttackType => AttackType.MachineGun;

        public override void OnSpawn()
        {
            attackTimer = attackDuration;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (attackTimer < attackDuration)
            {
                attackTimer += deltaTime;
                Idle();
            }
            else if (TrackTarget(ref target) || FindTarget(out target))
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

        private int counter;

        private void Idle()
        {
            if (counter < 1)
            {
                counter++;
                return;
            }

            counter = 0;
            lineRenderer.SetPosition(1, Vector3.zero);
        }

        private void Attack()
        {
            ShowAttackEffect();

            target.GetDamage(damage, AttackType);
            lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(target.LocalPosition));
        }

        private void ShowAttackEffect()
        {
            ObjectPool.Spawn<PoolObject>("MachineGunAttackEffect", attackPoint.position, attackPoint.rotation, Vector3.one).DelayUnspawn(0.5f);
        }
    }
}
