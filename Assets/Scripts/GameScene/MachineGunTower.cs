using UnityEngine;

namespace TowerDefense
{
    public class MachineGunTower : Tower
    {
        public float damage = 50f;
        public float attackDuration = 2f;

        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform gun = default;
        [SerializeField] private Transform shootPoint = default;

        private Enemy target;
        private float shootTimer;

        public override AttackType AttackType => AttackType.Normal;

        public override void OnUpdate()
        {
            if (shootTimer < attackDuration)
            {
                shootTimer += Time.deltaTime;
            }

            if (TrackTarget(ref target) || FindTarget(out target))
            {
                LookTarget();

                if (shootTimer >= attackDuration)
                {
                    shootTimer = 0f;
                    Shoot();
                }
            }
        }

        private void LookTarget()
        {
            Vector3 pos = target.Position;
            pos.y = 0f;
            turretBase.LookAt(pos);

            pos = (target.Position - gun.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);
        }

        private void Shoot()
        {
            ShowShootEffect();

            target.GetDamage(damage, AttackType);
        }

        private void ShowShootEffect()
        {
            ObjectPool.Spawn<PoolObject>("MuzzleEffect", shootPoint.position, shootPoint.rotation, Vector3.one).DelayUnspawn(1.5f);
        }
    }
}
