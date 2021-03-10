using System.IO;
using UnityEngine;
using LitJson;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        private void Awake()
        {
            string path = "Assets/StreamingAssets/LevelData/DefaultLevel.json";
            LevelData data = LevelData.CreateDefaultData();

            string str = JsonMapper.ToJson(data);
            File.WriteAllText(path, str);
        }
    }
}
