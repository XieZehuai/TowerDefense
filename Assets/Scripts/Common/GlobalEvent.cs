using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public struct OnChangePaths
    {
        public List<Vector3>[] paths;
    }

    public struct OnEnemyReach { }

    public struct StartGame { }

    public struct PauseGame { }

    public struct ContinueGame { }

    public struct ReplayGame { }

    public struct NextWave { }

    public struct OnReplay { }

    public struct NextWaveCountdown
    {
        public float countdown;
    }

    public struct OnUpdateHp
    {
        public int hp;
    }

    public struct OnUpdateCoins
    {
        public int coins;
    }

    public struct OnUpdateWave
    {
        public int waveCount;
    }
}
