using UnityEngine;

namespace TowerDefense
{
    public class LaserTower : Tower
    {
        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform shootPoint = default;
        [SerializeField] private Transform laser = default;

        private Enemy target; // 要攻击的目标敌人
        private LineRenderer lineRenderer;
        private static readonly Vector3 laserOriginPos = new Vector3(0f, 0f, -0.5f); // 激光的发射点

        private float attackAudioTimer = 0f;

        protected override void OnInstantiate()
        {
            lineRenderer = laser.GetComponent<LineRenderer>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (TrackTarget(ref target) || FindTarget(out target))
            {
                Attack(deltaTime);
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

        private void Attack(float deltaTime)
        {
            Vector3 pos = target.LocalPosition;
            pos.y = 0f;
            turretBase.LookAt(pos);

            pos = (target.LocalPosition - shootPoint.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);

            lineRenderer.SetPosition(1, laser.InverseTransformPoint(target.LocalPosition));

            float deltaDamage = Data.LevelData.damage * deltaTime;
            target.GetDamage(deltaDamage, Data.attackType);

            PlayAttackAudio();
        }

        private void PlayAttackAudio()
        {
            if (attackAudioTimer >= 0.1f)
            {
                attackAudioTimer = 0f;
                AudioManager.Instance.Play(transform.localPosition, Res.LaserTowerAttackAudio);
            }
            else
            {
                attackAudioTimer += Time.deltaTime;
            }
        }
    }
}
