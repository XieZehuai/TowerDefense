using System;
using UnityEngine;

namespace TowerDefense
{
    public abstract class SubStageManager
    {
        protected StageManager manager;

        public SubStageManager(StageManager stageManager)
        {
            manager = stageManager;
        }

        public virtual void OnUpdate()
        {
        }
    }
}
