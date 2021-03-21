using System;
using UnityEngine;

namespace TowerDefense
{
    public class MachineGunTower : Tower
    {
        public float damage = 50f;
        public float attackDuration = 2f;

        [SerializeField] private Transform turretBase = default;
        [SerializeField] private Transform turret = default;
        [SerializeField] private Transform shootPoint = default;

        private Enemy target;
        private GameObject shootEffect;
        private PoolObject hitEffect;
        private Coroutine hitCoroutine;
        private float shootTimer;

        public override AttackType AttackType => AttackType.Normal;

        private void Start()
        {
            shootEffect = ResourceManager.Load<GameObject>("MuzzleEffectPrefab");
            shootEffect = Instantiate(shootEffect, shootPoint);
            shootEffect.SetActive(false);
        }

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

            pos = (target.Position - shootPoint.position).normalized;
            pos += turret.position;
            turret.LookAt(pos);
        }

        private void Shoot()
        {
            ShowShootEffect();
            ShowTargetHitEffect();

            target.GetDamage(damage, AttackType);
        }

        private void ShowShootEffect()
        {
            shootEffect.gameObject.SetActive(false);
            shootEffect.gameObject.SetActive(true);
        }

        private void ShowTargetHitEffect()
        {
            Vector3 hitPosition = target.Position;
            hitPosition.y += 0.2f;

            if (hitEffect != null)
            {
                if (hitCoroutine != null) StopCoroutine(hitCoroutine);

                hitEffect.gameObject.SetActive(false);
                hitEffect.gameObject.SetActive(true);
                hitEffect.transform.localPosition = hitPosition;
            }
            else
            {
                hitEffect = ObjectPool.Spawn<PoolObject>("HitEffect", hitPosition);
                hitCoroutine = this.Invoke(() =>
                {
                    ObjectPool.Unspawn(hitEffect);
                    hitEffect = null;
                    hitCoroutine = null;
                }, 0.5f);
            }
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
    }
}
