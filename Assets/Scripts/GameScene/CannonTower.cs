using UnityEngine;

namespace TowerDefense
{
    public class CannonTower : Tower
    {
        [SerializeField] private float attackDuration = 1f;
        [SerializeField] private float shellBlastRadius = 1f;
        [SerializeField] private float shellDamage = 100f;

        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform attackPoint = default;

        private WarEntityManager warEntityManager;
        private float attackTimer;
        private float shootSpeed; // 炮弹初始发射速度

        public override AttackType AttackType => AttackType.Cannon;

        public override void OnSpawn()
        {
            float x = attackRange + 0.3f;
            float y = -attackPoint.position.y;
            shootSpeed = Mathf.Sqrt(Utils.GRAVITY * (y + Mathf.Sqrt(x * x + y * y)));
            attackTimer = attackDuration;
        }

        public override void OnUpdate()
        {
            if (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
            }
            else if (attackTimer >= attackDuration)
            {
                if (FindTarget(out Enemy target))
                {
                    Attack(target);
                    attackTimer = 0f;
                }
            }
        }

        public override void OnUnspawn()
        {
            warEntityManager = null;
        }

        public CannonTower SetWarEntityManager(WarEntityManager warEntityManager)
        {
            this.warEntityManager = warEntityManager;
            return this;
        }

        private void Attack(Enemy target)
        {
            Vector3 shootPos = attackPoint.position;
            Vector3 targetPos = target.LocalPosition;
            targetPos.y = 0f;

            Vector2 dir;
            dir.x = targetPos.x - shootPos.x;
            dir.y = targetPos.z - shootPos.z;
            float x = dir.magnitude;
            float y = -shootPos.y;
            dir /= x; // 目标方向的单位向量

            float g = Utils.GRAVITY; // 重力加速度
            float s = shootSpeed; // 炮塔发射时的初速度
            float s2 = s * s;
            float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
            float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
            float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
            float sinTheta = cosTheta * tanTheta;

            turretBase.localRotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.y));
            turret.localRotation = Quaternion.LookRotation(new Vector3(0f, tanTheta, Mathf.Abs(dir.y)));

            Vector3 velocity = new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y);
            warEntityManager.LaunchShell(shellBlastRadius, shellDamage, shootPos, targetPos, velocity);
        }
    }
}
