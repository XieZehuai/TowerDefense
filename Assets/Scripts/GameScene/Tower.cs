using UnityEngine;

namespace TowerDefense
{
    #region 炮塔的攻击类型
    public enum AttackType
    {
        MachineGun, // 普通机枪
        Laser, // 激光
        Cannon, // 加农炮
        Special, // 特殊攻击类型，造成特殊效果，不造成伤害
    }
    #endregion


    [SelectionBase]
    public abstract class Tower : PoolObject
    {
        [SerializeField] protected float attackRange = 2f; // 攻击范围

        /// <summary>
        /// 炮塔在地图上的X轴坐标
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 炮塔在地图上的Y轴坐标
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// 炮塔的攻击范围
        /// </summary>
        public float AttackRange => attackRange;

        /// <summary>
        /// 炮塔的攻击类型
        /// </summary>
        public abstract AttackType AttackType { get; }

        /// <summary>
        /// 炮塔的本地坐标
        /// </summary>
        public Vector3 LocalPosition => transform.localPosition;

        /// <summary>
        /// 设置炮塔坐标
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        public void SetCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 炮塔更新逻辑，每帧调用
        /// </summary>
        public virtual void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 寻找攻击范围内的敌人
        /// </summary>
        /// <param name="target">保存找到的敌人，如果找不到敌人，则赋值为null</param>
        /// <returns>找到返回true，失败返回false</returns>
        protected virtual bool FindTarget(out Enemy target)
        {
            if (Enemy.FindTargets(transform.localPosition, attackRange))
            {
                //target = Enemy.GetBestTarget(AttackType);
                target = Enemy.GetTarget(0);
                return target != null;
            }
            else
            {
                target = null;
                return false;
            }
        }

        /// <summary>
        /// 判断当前目标是否还存在
        /// </summary>
        /// <param name="target">当前的目标</param>
        /// <returns>存在返回true，失去目标返回false</returns>
        protected virtual bool TrackTarget(ref Enemy target)
        {
            if (target == null || !target.gameObject.activeSelf) return false;

            Vector3 a = LocalPosition;
            Vector3 b = target.LocalPosition;
            if (Vector3.Distance(a, b) > attackRange + 0.25f)
            {
                target = null;
                return false;
            }

            return true;
        }

        private void OnDrawGizmosSelected()
        {
            // 显示塔的攻击范围
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.localPosition + new Vector3(0f, 0.01f, 0f), attackRange);
        }
    }
}
