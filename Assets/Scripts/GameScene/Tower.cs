using UnityEngine;

namespace TowerDefense
{
    #region 炮塔的攻击类型
    public enum AttackType
    {
        Normal, // 普通机枪
        Laser, // 激光
        Cannon, // 加农炮
    }
    #endregion


    [SelectionBase]
    public abstract class Tower : PoolObject
    {
        public int x;
        public int y;
        public float attackRange = 2f;

        // 避免每次检测敌人时都分配内存
        protected static readonly Collider[] targetsBuffer = new Collider[100];

        /// <summary>
        /// 炮塔的攻击类型
        /// </summary>
        public abstract AttackType AttackType { get; }

        public Vector3 Position => transform.localPosition;

        public void SetCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 寻找攻击范围内的敌人
        /// </summary>
        /// <param name="target">保存找到的敌人，如果找不到敌人，则赋值为null</param>
        /// <returns>找到返回true，失败返回false</returns>
        protected virtual bool FindTarget(out Enemy target)
        {
            int hits = Physics.OverlapSphereNonAlloc(Position, attackRange, targetsBuffer, Utils.ENEMY_LAYER_MASK);

            if (hits > 0)
            {
                target = targetsBuffer[UnityEngine.Random.Range(0, hits)].GetComponent<Enemy>();
                return true;
            }

            target = null;
            return false;
        }

        /// <summary>
        /// 判断当前目标是否还存在
        /// </summary>
        /// <param name="target">当前的目标</param>
        /// <returns>存在返回true，失去目标返回false</returns>
        protected virtual bool TrackTarget(ref Enemy target)
        {
            if (target == null || !target.gameObject.activeSelf) return false;

            Vector3 a = Position;
            Vector3 b = target.Position;
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
