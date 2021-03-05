using System;
using System.IO;
using UnityEngine;

namespace TowerDefense
{
    public class MapManager : MonoSingleton<MapManager>
    {
        private string savePath; // 存档路径

        protected override void OnInit()
        {
        }

        private void Start()
        {
        }
    }
}
