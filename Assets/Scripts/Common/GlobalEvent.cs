/*
 * 各种用于类型事件系统传递的消息参数
 */
namespace TowerDefense
{
    public struct OnEnemyReach { }

    public struct OnEnemyDestroy
    {
        public int reward;
    }

    public struct OnReplay { }


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
