using System.IO;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
    public class ResourceTool : Editor
    {
        /// <summary>
        /// 读取所有Resources文件夹下的资源信息，提取出资源名以及路径生成配置
        /// 表，保存到StreamingAssets文件夹下
        /// </summary>
        [MenuItem("Tool/Generate Resource Config")]
        public static void GenerateResourceConfig()
        {
            string[] resFiles = AssetDatabase.FindAssets("t:object", new string[] { "Assets/Resources" });

            for (int i = 0; i < resFiles.Length; i++)
            {
                resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                string fileName = Path.GetFileNameWithoutExtension(resFiles[i]);
                string filePath = resFiles[i].Replace("Assets/Resources/", string.Empty).Split('.')[0];
                resFiles[i] = fileName + "=" + filePath;
            }

            File.WriteAllLines("Assets/StreamingAssets/ResourceConfig.txt", resFiles);
            AssetDatabase.Refresh();
        }

        [MenuItem("Tool/Reset Player Data")]
        public static void ResetPlayerData()
        {
            Debug.Log("重置玩家数据");

            string path = Application.streamingAssetsPath;
            path = Path.Combine(path, Utils.PLAYER_DATA_FILENAME);

            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(1);
                writer.Write(1);
            }
        }
    }
}
