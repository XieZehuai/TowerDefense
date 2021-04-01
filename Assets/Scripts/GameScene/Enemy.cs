using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class Enemy : PoolObject
    {
        #region 静态方法，用于寻找范围内的敌人
        // 用一个静态的数组来保存射线检测的结果，避免每次检测都分配新内存
        private static readonly Collider[] targetBuffer = new Collider[100];

        /// <summary>
        /// 检测到的范围内的敌人数量
        /// </summary>
        public static int TargetCount { get; private set; }

        /// <summary>
        /// 检测圆形范围内的敌人
        /// </summary>
        /// <param name="pos">原点</param>
        /// <param name="range">半径</param>
        /// <returns>检测到敌人返回true，检测不到返回false</returns>
        public static bool FindTargets(Vector3 pos, float range)
        {
            Vector3 top = pos;
            top.y += 3f;

            TargetCount = Physics.OverlapCapsuleNonAlloc(pos, top, range, targetBuffer, Utils.ENEMY_LAYER_MASK);

            return TargetCount > 0;
        }

        /// <summary>
        /// 根据索引获取检测到的敌人
        /// </summary>
        /// <param name="index">敌人的索引</param>
        public static Enemy GetTarget(int index)
        {
            Enemy target = targetBuffer[index].GetComponent<Enemy>();
            return target;
        }

        /// <summary>
        /// 根据攻击类型获取范围内最佳的敌人
        /// </summary>
        /// <param name="attackType">炮塔的攻击类型</param>
        public static Enemy GetBestTarget(AttackType attackType)
        {
            Enemy target = GetTarget(0);
            if (attackType == AttackType.Special) return target;

            // DamageConfig damageConfig = ConfigManager.Instance.DamageConfig;
            DamageConfig damageConfig = GameManager.Instance.DamageConfig;

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

        /// <summary>
        /// 对范围内的所有敌人执行相同的操作
        /// </summary>
        /// <param name="pos">原点</param>
        /// <param name="range">半径</param>
        /// <param name="action">要进行的操作</param>
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

        // 血量
        private Camera mainCamera;
        private Texture2D healthBarTexture;
        private Texture2D healthTexture;

        /// <summary>
        /// 敌人的本地坐标
        /// </summary>
        public Vector3 LocalPosition => transform.localPosition;

        /// <summary>
        /// 敌人正在移动的路径点
        /// </summary>
        public Vector3 NextWayPoint => path[curr];

        /// <summary>
        /// 敌人的护甲类型
        /// </summary>
        public ArmorType ArmorType => data.armorType;

        protected override void OnInstantiate()
        {
            mainCamera = Camera.main;
            healthBarTexture = ResourceManager.Load<Texture2D>("HealthBarTexture");
            healthTexture = ResourceManager.Load<Texture2D>("HealthTexture");
            Debug.Assert(healthBarTexture != null, "HealthBarTexture is null");
            Debug.Assert(healthTexture != null, "HealthTexture is null");
        }

        public override void OnSpawn()
        {
            // 让敌人移动时与路径点形成一定的偏移，避免全都沿着完全相同的路线走
            float x = UnityEngine.Random.Range(0f, 0.8f) - 0.4f;
            float z = UnityEngine.Random.Range(0f, 0.8f) - 0.4f;
            offset = new Vector3(x, 0.3f, z);
        }

        /// <summary>
        /// 设置敌人的数据
        /// </summary>
        /// <param name="data">敌人的数据</param>
        public Enemy SetData(EnemyData data)
        {
            this.data = data;
            currentHp = data.hp;
            currentSpeed = data.speed;

            return this;
        }

        /// <summary>
        /// 设置敌人移动的路径
        /// </summary>
        /// <param name="path">由所有路径点组成的List</param>
        /// <param name="moveToFirstWayPoint">是否直接移动到第一个路径点</param>
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

        /// <summary>
        /// 敌人受攻击
        /// </summary>
        /// <param name="damage">伤害</param>
        /// <param name="attackType">攻击类型</param>
        public void GetDamage(float damage, AttackType attackType)
        {
            // 计算实际造成的伤害
            // float actualDamage = ConfigManager.Instance.DamageConfig.GetDamage(damage, attackType, data.armorType);
            float actualDamage = GameManager.Instance.DamageConfig.GetDamage(damage, attackType, ArmorType);
            currentHp -= actualDamage;

            // 播放受击特效
            if (hitEffectTimer >= hitEffectDuration)
            {
                hitEffectTimer = 0f;
                ObjectPool.Spawn<Particle>("EnemyHitEffect").Follow(transform, new Vector3(0f, 0.2f, 0f))
                    .DelayUnspawn(0.5f);
            }
        }

        /// <summary>
        /// 让敌人减速
        /// </summary>
        /// <param name="duration">减速时长</param>
        /// <param name="rate">减速率</param>
        public void Decelerate(float duration, float rate)
        {
            isDecelerate = true;
            decelerateTimer = 0f;
            decelerateTime = duration;
            currentSpeed = Mathf.Min(data.speed * rate, currentSpeed);
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

        private void OnGUI()
        {
            // 绘制血量
            Vector2 healthBarSize = GUI.skin.label.CalcSize(new GUIContent(healthBarTexture)); // 计算图片尺寸
            Vector3 worldPos = transform.localPosition;
            worldPos.y += 0.5f; // 敌人头顶坐标
            Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos); // 转化成屏幕坐标
            screenPos.x -= healthBarSize.x * 0.5f;
            screenPos.y = Screen.height - screenPos.y + healthBarSize.y;
            float healthValue = healthTexture.width * currentHp / data.hp; // 计算生命条长度
            GUI.DrawTexture(new Rect(screenPos, healthBarSize), healthBarTexture);
            GUI.DrawTexture(new Rect(screenPos, new Vector2(healthValue, healthBarSize.y)), healthTexture);
        }
    }
}