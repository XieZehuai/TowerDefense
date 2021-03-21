using System;

namespace TowerDefense
{
    public abstract class SubStageManager : IDisposable
    {
        protected StageManager manager;

        public SubStageManager(StageManager stageManager)
        {
            manager = stageManager;
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void Dispose()
        {
            manager = null;
        }
    }
}
