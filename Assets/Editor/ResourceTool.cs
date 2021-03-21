using System.IO;
using UnityEditor;

namespace TowerDefense
{
    public class ResourceTool : Editor
    {
        /// <summary>
        /// 读取所有Resources文件夹下的资源信息，提取出资源名以及路径生成配置
        /// 表，保存到StreamingAssets文件夹下
        /// </summary>
        [MenuItem("Tool/Resource/Generate Resource Config")]
        public static void GenerateResourceConfig()
        {
            string[] resFiles = AssetDatabase.FindAssets("t:object", new string[] { "Assets/Resources" });

            for (int i = 0; i < resFiles.Length; i++)
            {
                resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                string fileName = Path.GetFileNameWithoutExtension(resFiles[i]);
                string filePath = resFiles[i].Replace("Assets/Resources/", string.Empty).Replace(".prefab", string.Empty);
                resFiles[i] = fileName + "=" + filePath;
            }

            File.WriteAllLines("Assets/StreamingAssets/ResourceConfig.txt", resFiles);
            AssetDatabase.Refresh();
        }
    }
}
