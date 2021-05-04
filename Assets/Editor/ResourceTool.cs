using System.IO;
using System.Text;
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
			string[] resFiles = AssetDatabase.FindAssets("t:object", new[] { "Assets/Resources" });

			// 根据资源名生成代码
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("namespace TowerDefense\n{\n\tpublic static class Res\n\t{\n\t\t");

			for (int i = 0; i < resFiles.Length; i++)
			{
				resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
				string fileName = Path.GetFileNameWithoutExtension(resFiles[i]);

				sb.AppendLine("\t\tpublic const string " + fileName + " = " + '"' + fileName + '"' + ';');

				string filePath = resFiles[i].Replace("Assets/Resources/", string.Empty).Split('.')[0];
				resFiles[i] = fileName + "=" + filePath;
			}

			sb.AppendLine("\t}\n}");

			File.WriteAllLines("Assets/StreamingAssets/ResourceConfig.txt", resFiles);
			File.WriteAllText("Assets/Scripts/ConfigAndData/Res.cs", sb.ToString());

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

				int maxStage = 1;
				writer.Write(maxStage);

				for (int i = 0; i < maxStage; i++)
				{
					writer.Write(0);
				}
			}
		}
	}
}