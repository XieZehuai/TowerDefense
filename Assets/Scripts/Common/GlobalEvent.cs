using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public struct OnChangePaths
    {
        public List<Vector3>[] paths;
    }

    public struct OnEnemyReach { }

    public struct OnEnemyDestroy
    {
        public int reward;
    }

    public struct OnReplay { }

    #region UI点击事件
    public struct StartGame { }

    public struct PauseGame { }

    public struct ContinueGame { }

    public struct ReplayGame { }

    public struct NextWave { }

    public struct ChangeGridType
    {
        public MapObjectType type;
    }

    public struct SaveMap { }

    public struct TogglePathIndicator { }
    #endregion

    #region 数据更新事件
    public struct UpdateNextWaveCountdown
    {
        public float countdown;
    }

    public struct UpdateHp
    {
        public int hp;
    }

    public struct UpdateCoins
    {
        public int coins;
    }

    public struct UpdateWaveCount
    {
        public int waveCount;
    }
    #endregion
}
