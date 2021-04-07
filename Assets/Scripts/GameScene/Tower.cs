using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense
{
    [SelectionBase]
    public abstract class Tower : PoolObject
    {
        private TowerData data; // 炮塔的数据

        /// <summary>
        /// 炮塔的数据
        /// </summary>
        public TowerData Data
        {
            get => data;
            set
            {
                data = value;
                OnSetData();
            }
        }

        public StageManager Manager { get; set; }

        /// <summary>
        /// 炮塔在地图上的X轴坐标
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 炮塔在地图上的Y轴坐标
        /// </summary>
        public int Y { get; private set; }

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
        /// 设置炮塔数据时调用
        /// </summary>
        protected virtual void OnSetData()
        {
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
            if (Enemy.FindTargets(transform.localPosition, Data.LevelData.attackRange))
            {
                target = Enemy.GetTarget(0);
                return target != null;
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

            Vector3 a = LocalPosition;
            Vector3 b = target.LocalPosition;
            if (Vector3.Distance(a, b) > Data.LevelData.attackRange + 0.25f)
            {
                target = null;
                return false;
            }

            return true;
        }

        private void OnMouseDown()
        {
            OnSelected();
        }

        private void OnSelected()
        {
            int cost = data.GetNextLevelCost();

            Particle particle = ObjectPool.Spawn<Particle>("TowerAttackRangeEffect", transform.localPosition);
            particle.SetFloat("AttackRange", data.LevelData.attackRange);

            UIManager.Instance.Open<UITowerOption>(new UITowerOptionData
            {
                position = LocalPosition,
                onUpgradeBtnClick = LevelUp,
                onSellBtnClick = Sell,
                onHide = () => ObjectPool.Unspawn(particle),
                upgradePrice = cost,
                canUpgrade = cost != -1 && Manager.Coins >= cost,
                sellPrice = data.GetTotalCost() / 2
            }, UILayer.Background);
        }

        private void LevelUp()
        {
            int cost = data.GetNextLevelCost();

            if (cost != -1 && Manager.Coins >= cost)
            {
                data.LevelUp();
                Manager.Coins -= cost;
            }
        }

        private void Sell()
        {
            Manager.Coins += data.GetTotalCost() / 2;
            Manager.TowerManager.RemoveTower(this);
        }

        public override void OnUnspawn()
        {
            Manager = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (Data.Level > 0)
            {
                // 显示塔的攻击范围
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.localPosition + new Vector3(0f, 0.01f, 0f), data.LevelData.attackRange);
            }
        }
    }
}