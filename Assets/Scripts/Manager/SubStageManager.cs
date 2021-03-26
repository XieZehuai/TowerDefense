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

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void Dispose()
        {
            manager = null;
        }
    }
}
