using UnityEngine;

namespace TowerDefense
{
    public class LaserTower : Tower
    {
        public float damagePerSecond = 100f; // 每秒钟造成的伤害

        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform upGun = default;
        [SerializeField] private Transform laser = default;

        private Enemy target; // 要攻击的目标敌人
        private LineRenderer lineRenderer;
        private static readonly Vector3 laserOriginPos = new Vector3(0f, 0f, -0.5f); // 激光的发射点

        public override AttackType AttackType => AttackType.Laser;

        private void Awake()
        {
            lineRenderer = laser.GetComponent<LineRenderer>();
        }

        public override void OnUpdate()
        {
            if (TrackTarget(ref target) || FindTarget(out target))
            {
                Shoot();
            }
            else
            {
                Idle();
            }
        }

        private void Idle()
        {
            lineRenderer.SetPosition(1, laserOriginPos);
        }

        private void Shoot()
        {
            Vector3 pos = target.LocalPosition;
            pos.y = 0f;
            turretBase.LookAt(pos);

            pos = (target.LocalPosition - upGun.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);

            lineRenderer.SetPosition(1, laser.InverseTransformPoint(target.LocalPosition));

            float deltaDamage = damagePerSecond * Time.deltaTime;
            target.GetDamage(deltaDamage, AttackType);
        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.DrawLine(transform.localPosition, target.LocalPosition);
            }
        }
    }
}
