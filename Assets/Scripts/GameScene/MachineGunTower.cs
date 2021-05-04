using UnityEngine;

namespace TowerDefense
{
    public class MachineGunTower : Tower
    {
        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform gun = default;
        [SerializeField] private Transform attackPoint = default;
        [SerializeField] private LineRenderer lineRenderer = default;

        private Enemy target;
        private float attackTimer;
        private int counter;

        protected override void OnSetData()
        {
            attackTimer = Data.LevelData.attackDuration;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (attackTimer < Data.LevelData.attackDuration)
            {
                attackTimer += deltaTime;
                Idle();
            }
            else if (TrackTarget(ref target) || FindTarget(out target))
            {
                LookTarget();

                if (attackTimer >= Data.LevelData.attackDuration)
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

            target.GetDamage(Data.LevelData.damage, Data.attackType);
            lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(target.LocalPosition));

            AudioManager.Instance.Play(transform.localPosition, Res.MachineGunTowerAttackAudio);
        }

        private void ShowAttackEffect()
        {
            //ObjectPool.Spawn<PoolObject>("MachineGunAttackEffect", attackPoint.position, attackPoint.rotation, Vector3.one).DelayUnspawn(0.5f);
            ObjectPool.Spawn<PoolObject>(Res.MachineGunAttackEffectPrefab, attackPoint.position,
                attackPoint.rotation, Vector3.one).DelayUnspawn(0.5f);
        }
    }
}
