using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 可用对象池管理的对象
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        /// <summary>
        /// 对象的标签，对应所在的池子
        /// </summary>
        public string Tag { get; private set; }

        private bool isDestroy = false;

        /// <summary>
        /// 在预设被加载并实例化时调用
        /// </summary>
        /// <param name="tag">对象所在池子标签</param>
        /// <param name="destroyOnLoad">是否在切换场景时销毁当前对象，默认为true</param>
        public void Instantiate(string tag, bool destroyOnLoad = true)
        {
            Tag = tag;
            OnInstantiate();

            if (!destroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// 实例化时调用
        /// </summary>
        protected virtual void OnInstantiate() { }

        /// <summary>
        /// 从对象池中取出时调用
        /// </summary>
        public virtual void OnSpawn() { }

        /// <summary>
        /// 回收进对象池时调用
        /// </summary>
        public virtual void OnUnspawn() { }

        /// <summary>
        /// 从对象池中移除时调用
        /// </summary>
        public virtual void OnReclaim() { }

        /// <summary>
        /// 让对象池回收当前对象
        /// </summary>
        public void UnspawnSelf()
        {
            ObjectPool.Unspawn(this);
        }

        /// <summary>
        /// 延迟一段时间后回收当前对象
        /// </summary>
        /// <param name="delay">延迟时间</param>
        public void DelayUnspawn(float delay)
        {
            this.Invoke(() =>
            {
                ObjectPool.Unspawn(this);
            }, delay);
        }

        /// <summary>
        /// 延迟一定条件后回收当前对象
        /// </summary>
        /// <param name="condition">延迟条件</param>
        public void DelayUnspawn(YieldInstruction condition)
        {
            this.Invoke(() =>
            {
                ObjectPool.Unspawn(this);
            }, condition);
        }

        /// <summary>
        /// 销毁当前对象
        /// </summary>
        public void Destroy()
        {
            if (isDestroy)
            {
                Debug.LogError("物体已经被回收" + Tag);
                return;
            }

            isDestroy = true;
            ObjectPool.Clear(this);
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}
