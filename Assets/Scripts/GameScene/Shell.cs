using UnityEngine;

namespace TowerDefense
{
    public class Shell : WarEntity
    {
        private float blastRadius;
        private float damage;
        private Vector3 attackPos;
        private Vector3 targetPos;
        private Vector3 velocity;
        private float timer;

        public override void OnSpawn()
        {
            timer = 0f;
        }

        public void Init(float blastRadius, float damage, Vector3 attackPos, Vector3 targetPos, Vector3 velocity)
        {
            this.blastRadius = blastRadius;
            this.damage = damage;
            this.attackPos = attackPos;
            this.targetPos = targetPos;
            this.velocity = velocity;
        }

        public override bool OnUpdate()
        {
            timer += Time.deltaTime;
            Vector3 pos = attackPos + velocity * timer;
            pos.y -= 0.5f * Utils.GRAVITY * timer * timer;

            if (pos.y <= 0f)
            {
                Blast();
                return false;
            }

            transform.localPosition = pos;
            return true;
        }

        private void Blast()
        {
            Enemy.FillBuffer(targetPos, blastRadius);
            for (int i = 0; i < Enemy.BufferCount; i++)
            {
                Enemy.GetTarget(i).GetDamage(damage, AttackType.Cannon); ;
            }
            ObjectPool.Spawn("ExplosionEffect", transform.localPosition, Quaternion.identity, Vector3.one).DelayUnspawn(1.5f);
        }
    }
}
