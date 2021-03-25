using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class Enemy : PoolObject
    {
        #region 静态方法，用于寻找范围内的敌人
        private static readonly Collider[] buffer = new Collider[100];

        public static int BufferCount { get; private set; }

        public static bool FillBuffer(Vector3 pos, float range)
        {
            Vector3 top = pos;
            top.y += 3f;

            BufferCount = Physics.OverlapCapsuleNonAlloc(pos, top, range, buffer, Utils.ENEMY_LAYER_MASK);

            return BufferCount > 0;
        }

        public static Enemy GetTarget(int index)
        {
            if (index >= BufferCount)
            {
                Debug.LogError("索引不能大于Buffer大小" + index + " " + BufferCount);
                return null;
            }

            Enemy target = buffer[index].GetComponent<Enemy>();
            return target;
        }

        public static Enemy GetTarget(AttackType attackType)
        {
            // TODO: 寻找最合适的敌人
            return null;
        }
        #endregion

        private EnemyData data;
        private float currentHp;
        private float currentSpeed;

        private List<Vector3> path; // 路径点
        private int curr; // 当前目标路径点的索引
        private Vector3 originPos;
        private float distance; // 当前物体距离目标点的距离
        private float progress;
        private Vector3 height = new Vector3(0f, 0.3f, 0f); // 飞行的高度
        private float hitEffectDuration = 0.1f;
        private float hitEffectTimer;

        public Vector3 LocalPosition => transform.localPosition;

        public Vector3 NextWayPoint => path[curr];

        public string Name => data.name;

        public Enemy SetData(EnemyData data)
        {
            this.data = data;
            currentHp = data.hp;
            currentSpeed = data.speed;

            return this;
        }

        public Enemy SetPath(List<Vector3> path, bool moveToFirstWayPoint)
        {
            if (path.Count <= 1) Debug.LogError("路径长度太短");

            this.path = path;

            if (moveToFirstWayPoint)
            {
                originPos = path[0];
                transform.localPosition = path[0] + height;
                transform.localRotation = Quaternion.LookRotation(path[1] - path[0]);
                distance = Vector3.Distance(path[1], path[0]);
                curr = 1;
            }
            else
            {
                originPos = transform.localPosition;
                distance = Vector3.Distance(transform.localPosition, path[0]);
                transform.localPosition += height;
                curr = 0;
            }

            return this;
        }

        public bool OnUpdate()
        {
            if (hitEffectTimer < hitEffectDuration)
            {
                hitEffectTimer += Time.deltaTime;
            }

            if (currentHp <= 0)
            {
                TypeEventSystem.Send(new OnEnemyDestroy { reward = data.reward });
                ObjectPool.Spawn("ExplosionEffect", transform.localPosition, Quaternion.identity, Vector3.one).DelayUnspawn(1.5f);
                return false;
            }

            return Move();
        }

        public void GetDamage(float damage, AttackType attackType)
        {
            float actualDamage = ConfigManager.Instance.DamageConfig.GetDamage(damage, attackType, data.armorType);
            currentHp -= actualDamage;

            if (hitEffectTimer >= hitEffectDuration)
            {
                hitEffectTimer = 0f;

                ObjectPool.Spawn<Particle>("HitEffect").Follow(transform, new Vector3(0f, 0.2f, 0f)).DelayUnspawn(0.5f);
            }
        }

        public Vector3 GetNextWayPoint()
        {
            return path[curr];
        }

        private bool Move()
        {
            if (curr >= path.Count)
            {
                TypeEventSystem.Send(new OnEnemyReach());
                return false;
            }

            progress += Time.deltaTime * currentSpeed;
            transform.localPosition = Vector3.Lerp(originPos, path[curr], progress / distance) + height;

            if (progress >= distance)
            {
                originPos = path[curr];
                curr++;
                progress = 0f;

                if (curr < path.Count)
                {
                    distance = Vector3.Distance(path[curr], path[curr - 1]);
                    transform.localRotation = Quaternion.LookRotation(path[curr] - path[curr - 1]);
                }
            }

            return true;
        }
    }
}
