using UnityEngine;

namespace TowerDefense
{
    public class CannonTower : Tower
    {
        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform attackPoint = default;

        private WarEntityManager warEntityManager;
        private float attackTimer;
        private float shootSpeed; // 炮弹初始发射速度

        protected override void OnSetData()
        {
            float x = Data.LevelData.attackRange + 0.3f;
            float y = -attackPoint.position.y;
            shootSpeed = Mathf.Sqrt(Utils.GRAVITY * (y + Mathf.Sqrt(x * x + y * y)));
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
            Debug.Log("Attack Pos" + shootPos);
            Debug.Log("Target Pos " + targetPos);
            targetPos.y = 0f;

            // XZ平面上目标点的方向
            Vector2 dir = new Vector2(targetPos.x - shootPos.x, targetPos.z - shootPos.z);
            float x = dir.magnitude;　// 取模
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
            Vector3 rot = new Vector3(0f, tanTheta, Mathf.Abs(dir.y));
            Debug.Log("Rot " + rot);
            turret.localRotation = Quaternion.LookRotation(rot);

            Vector3 velocity = new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y);
            warEntityManager.LaunchShell(Data.LevelData.shellBlastRadius, Data.LevelData.damage, shootPos, targetPos, velocity);
        }
    }
}