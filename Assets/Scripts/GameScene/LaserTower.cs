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
        private PoolObject hitEffect; // 敌人受到攻击的特效

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
            Vector3 pos = target.Position;
            pos.y = 0f;
            turretBase.LookAt(pos);

            pos = (target.Position - upGun.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);

            Vector3 hitPosition = target.Position;
            hitPosition.y += 0.2f;
            lineRenderer.SetPosition(1, laser.InverseTransformPoint(hitPosition));

            if (hitEffect == null)
            {
                hitEffect = ObjectPool.Spawn<PoolObject>("HitEffect", hitPosition);
                this.Invoke(() =>
                {
                    ObjectPool.Unspawn(hitEffect);
                    hitEffect = null;
                }, 0.5f);
            }
            else
            {
                hitEffect.transform.localPosition = hitPosition;
            }

            float deltaDamage = damagePerSecond * Time.deltaTime;
            target.GetDamage(deltaDamage, AttackType);
        }

        public override void OnUnspanw()
        {
            StopAllCoroutines();
            if (hitEffect != null)
            {
                ObjectPool.Unspawn(hitEffect);
            }
        }

        public override void OnReclaim()
        {
            StopAllCoroutines();
            if (hitEffect != null)
            {
                ObjectPool.Unspawn(hitEffect);
            }
        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.DrawLine(transform.localPosition, target.Position);
            }
        }
    }
}
