using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class Enemy : PoolObject
    {
        #region 静态方法，用于寻找范围内的敌人
        private static readonly Collider[] targetBuffer = new Collider[100];

        public static int TargetCount { get; private set; }

        public static bool FindTargets(Vector3 pos, float range)
        {
            Vector3 top = pos;
            top.y += 3f;

            TargetCount = Physics.OverlapCapsuleNonAlloc(pos, top, range, targetBuffer, Utils.ENEMY_LAYER_MASK);

            return TargetCount > 0;
        }

        public static Enemy GetTarget(int index)
        {
            Enemy target = targetBuffer[index].GetComponent<Enemy>();
            return target;
        }

        public static Enemy GetBestTarget(AttackType attackType)
        {
            Enemy target = GetTarget(0);
            if (attackType == AttackType.Special) return target;

            DamageConfig damageConfig = ConfigManager.Instance.DamageConfig;
            int priority = damageConfig.GetArmorPriority(attackType, target.ArmorType);
            if (priority == 0) return target;

            for (int i = 1; i < TargetCount; i++)
            {
                Enemy tempTarget = GetTarget(i);
                int tempPriority = damageConfig.GetArmorPriority(attackType, tempTarget.ArmorType);

                if (tempPriority < priority)
                {
                    if (tempPriority == 0) return tempTarget;

                    target = tempTarget;
                    priority = tempPriority;
                }
            }

            return target;
        }

        public static void AttackAll(Vector3 pos, float range, Action<Enemy> action)
        {
            FindTargets(pos, range);

            for (int i = 0; i < TargetCount; i++)
            {
                action?.Invoke(GetTarget(i));
            }
        }
        #endregion

        private EnemyData data;
        private float currentHp; // 当前生命值
        private float currentSpeed; // 当前移动速度

        private List<Vector3> path; // 移动路径
        private int curr; // 当前路径点的索引
        private Vector3 originPos; // 初始位置
        private float distance; // 当前物体距离目标点的距离
        private float progress;
        private Vector3 offset; // 移动时距离目标点的偏移量
        private float hitEffectDuration = 0.1f;
        private float hitEffectTimer;

        private float decelerateTime;
        private float decelerateTimer;
        private bool isDecelerate;

        public Vector3 LocalPosition => transform.localPosition;

        public Vector3 NextWayPoint => path[curr];

        public ArmorType ArmorType => data.armorType;

        public override void OnSpawn()
        {
            float x = UnityEngine.Random.Range(0f, 0.8f) - 0.4f;
            float z = UnityEngine.Random.Range(0f, 0.8f) - 0.4f;
            offset = new Vector3(x, 0.3f, z);
        }

        public Enemy SetData(EnemyData data)
        {
            this.data = data;
            currentHp = data.hp;
            currentSpeed = data.speed;

            return this;
        }

        public void SetPath(List<Vector3> path, bool moveToFirstWayPoint)
        {
            if (path.Count <= 1) Debug.LogError("路径长度太短");

            this.path = path;

            if (moveToFirstWayPoint)
            {
                originPos = path[0];
                transform.localPosition = path[0] + offset;
                transform.localRotation = Quaternion.LookRotation(path[1] - path[0]);
                distance = Vector3.Distance(path[1], path[0]);
                curr = 1;
            }
            else
            {
                originPos = transform.localPosition;
                distance = Vector3.Distance(transform.localPosition, path[0]);
                transform.localPosition += offset;
                curr = 0;
            }
        }

        public bool OnUpdate(float deltaTime)
        {
            if (hitEffectTimer < hitEffectDuration)
            {
                hitEffectTimer += deltaTime;
            }

            if (currentHp <= 0)
            {
                TypeEventSystem.Send(new OnEnemyDestroy { reward = data.reward });
                ObjectPool.Spawn("EnemyDestroyEffect", transform.localPosition, Quaternion.identity, Vector3.one)
                    .DelayUnspawn(1.5f);
                return false;
            }

            if (isDecelerate)
            {
                decelerateTimer += deltaTime;

                if (decelerateTimer >= decelerateTime)
                {
                    currentSpeed = data.speed;
                    isDecelerate = false;
                    decelerateTimer = 0f;
                    decelerateTime = 0f;
                }
            }

            return Move(deltaTime);
        }

        public void GetDamage(float damage, AttackType attackType)
        {
            float actualDamage = ConfigManager.Instance.DamageConfig.GetDamage(damage, attackType, data.armorType);
            currentHp -= actualDamage;

            if (hitEffectTimer >= hitEffectDuration)
            {
                hitEffectTimer = 0f;

                ObjectPool.Spawn<Particle>("EnemyHitEffect").Follow(transform, new Vector3(0f, 0.2f, 0f))
                    .DelayUnspawn(0.5f);
            }
        }

        public void Decelerate(float duration, float rate)
        {
            isDecelerate = true;
            decelerateTimer = 0f;
            decelerateTime = duration;
            currentSpeed = Mathf.Min(data.speed * rate, currentSpeed);
        }

        public Vector3 GetNextWayPoint()
        {
            return path[curr];
        }

        private bool Move(float deltaTime)
        {
            if (curr >= path.Count)
            {
                TypeEventSystem.Send(new OnEnemyReach());
                return false;
            }

            progress += deltaTime * currentSpeed;
            transform.localPosition = Vector3.Lerp(originPos, path[curr], progress / distance) + offset;

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

        public override void OnUnspawn()
        {
            isDecelerate = false;
            decelerateTimer = 0f;
            decelerateTime = 0f;
        }
    }
}