using System;

namespace TowerDefense
{
    public abstract class SubStageManager : IDisposable
    {
        protected StageManager manager;

        protected SubStageManager(StageManager stageManager)
        {
            manager = stageManager;
        }
        
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="deltaTime">每帧的时间</param>
        public virtual void OnUpdate(float deltaTime)
        {
        }

        public void Dispose()
        {
            OnDispose();
            manager = null;
        }

        /// <summary>
        /// 在手动释放资源时调用
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}
