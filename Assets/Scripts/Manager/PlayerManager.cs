using System.IO;
using UnityEngine;

namespace TowerDefense
{
    public static class PlayerManager
    {
        public struct PlayerData
        {
            public int Stage { get; set; }

            public int MaxStage { get; set; }

            public PlayerData(int stage, int maxStage)
            {
                Stage = stage;
                MaxStage = maxStage;
            }
        }


        private static PlayerData data;
        public static PlayerData Data => data;

        static PlayerManager()
        {
            Load();
        }

        public static void StageSuccess()
        {
            if (data.MaxStage == data.Stage)
            {
                data.MaxStage++;
            }

            Save();
        }

        public static void NextStage()
        {
            if (data.Stage >= data.MaxStage)
            {
                Debug.LogError("已经到达最大关卡数");
                data.Stage++;
                data.MaxStage = data.Stage;
            }
            else
            {
                data.Stage++;
            }

            Save();
        }

        public static void Save()
        {
            Debug.Log("保存玩家数据");

            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, Utils.PLAYER_DATA_FILENAME);

            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(Data.Stage);
                writer.Write(Data.MaxStage);
            }
        }

        private static void Load()
        {
            Debug.Log("加载玩家数据");

            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, Utils.PLAYER_DATA_FILENAME);

            if (File.Exists(path))
            {
                using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int stage = reader.ReadInt32();
                    int maxStage = reader.ReadInt32();
                    data = new PlayerData(stage, maxStage);
                }
            }
            else
            {
                data = new PlayerData(1, 1);
                Save();
            }
        }
    }
}
