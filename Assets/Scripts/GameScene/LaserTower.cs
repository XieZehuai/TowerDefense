using System;
using UnityEngine;

namespace TowerDefense
{
    [SelectionBase]
    public class LaserTower : Tower
    {
        public float damagePerSecond = 30f; // 每秒钟造成的伤害

        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform upGun = default;
        [SerializeField] private Transform laser = default;

        private Enemy target; // 要攻击的目标敌人
        private LineRenderer lineRenderer;
        private static readonly Vector3 laserOriginPos = new Vector3(0f, 0f, -0.5f); // 激光的发射点
        private Vector3 laserOffset = laserOriginPos; // 激光目标点的偏移量

        public override AttackType AttackType => AttackType.Laser;

        private void Awake()
        {
            lineRenderer = laser.GetComponent<LineRenderer>();
        }

        public override void OnUpdate()
        {
            if (TrackTarget(ref target) || FindTarget())
            {
                Shoot();
            }
            else
            {
                Idle();
            }
        }

        private bool FindTarget()
        {
            if (FindTarget(out target))
            {
                laserOffset = new Vector3(0f, 0.1f, 0f);
                return true;
            }

            laserOffset = laserOriginPos;
            return false;
        }

        private void Idle()
        {
            lineRenderer.SetPosition(1, laserOriginPos);
        }

        private void Shoot()
        {
            Vector3 pos = target.Position;
            pos.y = 0f;
            turretBase.LookAt(pos);

            pos = (target.Position - upGun.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);

            lineRenderer.SetPosition(1, laser.InverseTransformPoint(target.Position));

            float deltaDamage = damagePerSecond * Time.deltaTime;
            target.GetDamage(deltaDamage, AttackType);
        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.DrawLine(transform.localPosition, target.Position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.localPosition;
            pos.y += 0.01f;
            Gizmos.DrawWireSphere(pos, attackRange);
        }
    }
}
