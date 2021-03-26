using System;
using UnityEngine;

namespace TowerDefense
{
    public abstract class WarEntity : PoolObject
    {
        public virtual bool OnUpdate(float deltaTime)
        {
            return true;
        }
    }
}
