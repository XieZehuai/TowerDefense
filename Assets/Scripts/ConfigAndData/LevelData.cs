using System;
using System.Collections.Generic;

namespace TowerDefense
{
    [Serializable]
    public class LevelData
    {
        // 初始生命以及金币
        public int playerHp;
        public int coins;

        // 地图数据
        public int mapWidth;
        public int mapHeight;
        public MapObjectType[] mapData;

        // 敌人数据
        public float waveInterval; // 每波的间隔
        public float spawnInterval; // 每个敌人生成的间隔
        public Dictionary<int, int>[] waveData; // 每波敌人的配置

        public static LevelData CreateDefaultData()
        {
            LevelData data = new LevelData();

            data.playerHp = 10;
            data.coins = 200;

            data.mapWidth = 10;
            data.mapHeight = 10;
            data.mapData = new MapObjectType[data.mapWidth * data.mapHeight];

            for (int i = 0; i < data.mapData.Length; i++)
            {
                data.mapData[i] = MapObjectType.Road;
            }
            data.mapData[5] = MapObjectType.SpawnPoint;
            data.mapData[data.mapData.Length - 1] = MapObjectType.Destination;

            data.waveInterval = 5f;
            data.spawnInterval = 1f;
            data.waveData = new Dictionary<int, int>[5];

            data.waveData[0] = new Dictionary<int, int> { { 0, 6 }, { 3, 4 }, { 6, 2 }, };
            data.waveData[1] = new Dictionary<int, int> { { 0, 4 }, { 1, 4 }, { 3, 5 }, { 6, 2 }, };
            data.waveData[2] = new Dictionary<int, int> { { 0, 4 }, { 1, 4 }, { 3, 2 }, { 4, 2 }, { 6, 1 }, { 7, 1 } };
            data.waveData[3] = new Dictionary<int, int> { { 1, 4 }, { 2, 4 }, { 4, 2 }, { 5, 2 }, { 7, 1 }, { 8, 1 } };
            data.waveData[4] = new Dictionary<int, int> { { 2, 8 }, { 1, 4 }, { 5, 4 }, { 8, 2 } };

            return data;
        }
    }
}