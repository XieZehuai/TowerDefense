using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TowerDefense
{
    public static class ResourceManager
    {
        /// <summary>
        /// StreamingAssets文件夹在当前平台下的实际路径
        /// </summary>
        public static readonly string streamingAssetsPath;

        private static Dictionary<string, string> configMap; // 保存资源名到路径的映射

        static ResourceManager()
        {
            streamingAssetsPath = GetStreamingAssetsPath();

            string url = streamingAssetsPath + "ResourceConfig.txt";
            string fileContent = GetFileContent(url);
            BuildConfigMap(fileContent);
        }

        /// <summary>
        /// 根据传入的资源名称，加载资源（必须是Resources路径下的资源）
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <returns>加载成功返回资源文件，失败返回null</returns>
        public static T Load<T>(string name) where T : UnityEngine.Object
        {
            if (!configMap.ContainsKey(name))
            {
                Debug.LogError("找不到资源文件对应的路径 " + name);
                return null;
            }

            string path = configMap[name];
            return Resources.Load<T>(path);
        }

        /// <summary>
        /// 根据传入的url路径，获取文本内容
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>资源内容</returns>
        public static string GetFileContent(string url)
        {
            WWW www = new WWW(url);

            while (true)
            {
                if (www.isDone)
                {
                    return www.text;
                }
            }
        }

        /// <summary>
        /// 生成 文件名-路径 的配置表
        /// </summary>
        /// <param name="content">配置表内容</param>
        private static void BuildConfigMap(string content)
        {
            configMap = new Dictionary<string, string>();

            using (StringReader reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split('=');
                    if (!configMap.ContainsKey(values[0]))
                    {
                        configMap.Add(values[0], values[1]);
                    }
                }
            }
        }

        /// <summary>
        /// 获取StreamingAssets文件夹在各个平台下的路径
        /// </summary>
        private static string GetStreamingAssetsPath()
        {
            string path;

#if UNITY_EDITOR || UNITY_STANDALONE
            path = "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
            path = "file://" + Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
            path = "jar:file://" + Application.dataPath + "/!/assets/";
#endif

            return path;
        }
    }
}
