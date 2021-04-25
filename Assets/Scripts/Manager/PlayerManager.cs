using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public static class PlayerManager
    {
        #region 玩家数据类
        /// <summary>
        /// 玩家数据
        /// </summary>
        public struct PlayerData
        {
            /// <summary>
            /// 当前正在玩的关卡
            /// </summary>
            public int CurrentStage { get; set; }

            /// <summary>
            /// 玩家已解锁的最大关卡
            /// </summary>
            public int ReachStage { get; set; }

            /// <summary>
            /// 每一个关卡的评分
            /// </summary>
            public List<int> StageData { get; set; }

            /// <summary>
            /// 游戏内的最大关卡数量
            /// </summary>
            public int MaxStage => StageData.Count;

            public PlayerData(int currentStage, int reachStage, List<int> stageData)
            {
                CurrentStage = currentStage;
                ReachStage = reachStage;
                StageData = stageData;
            }
        }
        #endregion


        private static PlayerData data;
        public static PlayerData Data => data;

        /// <summary>
        /// 关卡成功结算，判断是否解锁新关卡以及更新关卡评分等
        /// </summary>
        /// <param name="starCount">关卡评分</param>
        public static void StageSuccess(int starCount)
        {
            if (starCount > GetStageStar(data.CurrentStage))
            {
                SetStageStar(data.CurrentStage, starCount);
            }

            // 如果当前通过的关卡是已解锁的最大关卡，就解锁下一关
            if (data.CurrentStage == data.ReachStage)
            {
                data.ReachStage++;
                // 解锁的关卡超过的最大关卡数，添加一个新关卡的数据
                if (data.ReachStage > data.MaxStage)
                {
                    Debug.Log("抵达最大关卡数");
                    data.StageData.Add(0);
                }

                // 更新完后保存数据
            }

            SavePlayerData();
        }

        /// <summary>
        /// 改变当前正在玩的关卡数
        /// </summary>
        /// <param name="stage">目标关卡数</param>
        public static void ChangeCurrentStage(int stage)
        {
            if (stage > data.ReachStage)
            {
                Debug.LogError("当前关卡还没解锁: " + stage);
            }
            else
            {
                data.CurrentStage = stage;
            }
        }

        /// <summary>
        /// 进入下一关，更新当前关卡、解锁关卡以及最大关卡等数据
        /// </summary>
        public static void NextStage()
        {
            data.CurrentStage++;
            SavePlayerData();
        }

        /// <summary>
        /// 获取关卡评分
        /// </summary>
        /// <param name="stage">关卡数</param>
        /// <returns>关卡评分</returns>
        public static int GetStageStar(int stage)
        {
            return data.StageData[stage - 1];
        }

        /// <summary>
        /// 设置关卡评分
        /// </summary>
        /// <param name="stage">关卡数</param>
        /// <param name="starCount">关卡评分</param>
        public static void SetStageStar(int stage, int starCount)
        {
            data.StageData[stage - 1] = starCount;
        }

        /// <summary>
        /// 保存玩家数据，将数据写入存档文件中
        /// </summary>
        public static void SavePlayerData()
        {
            Debug.Log("保存玩家数据");

            // 获取存档路径
            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, Utils.PLAYER_DATA_FILENAME);

            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(data.CurrentStage);
                writer.Write(data.ReachStage);
                writer.Write(data.MaxStage);

                for (int i = 0; i < data.MaxStage; i++)
                {
                    writer.Write(data.StageData[i]);
                }
            }
        }

        /// <summary>
        /// 加载玩家存档数据，如果没有存档，则新建一个存档并保存
        /// </summary>
        public static void LoadPlayerData()
        {
            Debug.Log("加载玩家数据");

            // 获取存档路径
            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, Utils.PLAYER_DATA_FILENAME);

            // 判断是否存在存档文件
            if (File.Exists(path))
            {
                // 存在存档文件，读取玩家数据
                using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int currentStage = reader.ReadInt32();
                    int reachStage = reader.ReadInt32();
                    int maxStage = reader.ReadInt32();

                    List<int> stageData = new List<int>();
                    for (int i = 0; i < maxStage; i++)
                    {
                        stageData.Add(reader.ReadInt32());
                    }

                    data = new PlayerData(currentStage, reachStage, stageData);
                }
            }
            else
            {
                // 不存在存档文件，生成新玩家数据并保存
                data = CreateNewData();
                SavePlayerData();
            }
        }

        /// <summary>
        /// 创建新的玩家数据
        /// </summary>
        private static PlayerData CreateNewData()
        {
            int currentStage = 1;
            int reachStage = 1;
            int maxStage = GameManager.Instance.MaxStageCount;
            List<int> stageData = new List<int>();
            for (int i = 0; i < maxStage; i++)
            {
                stageData.Add(0);
            }

            PlayerData data = new PlayerData(currentStage, reachStage, stageData);
            return data;
        }
    }
}
