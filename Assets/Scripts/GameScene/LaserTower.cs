using System;
using UnityEngine;

namespace TowerDefense
{
    [SelectionBase]
    public class LaserTower : Tower
    {
        public float damagePerSecond = 30f;

        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform gun = default;

        private Enemy target;
        private LineRenderer lineRenderer;
        private static readonly Vector3 laserOriginPos = new Vector3(0f, 0f, -0.5f); // 激光的发射点
        private Vector3 laserOffset = laserOriginPos; // 激光目标点的偏移量

        public override AttackType AttackType => AttackType.Laser;

        private void Awake()
        {
            lineRenderer = gun.GetComponent<LineRenderer>();
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
            turret.LookAt(target.Position);
            lineRenderer.SetPosition(1, gun.InverseTransformPoint(target.Position + laserOffset));
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
