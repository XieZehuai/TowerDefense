using System;
using UnityEngine;

namespace TowerDefense
{
    public class MapManager : MonoSingleton<MapManager>
    {
        public MapObjectType type = MapObjectType.Empty;

        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private int cellSize = 1;
        [SerializeField] private Transform gridParent;

        private Map map;

        protected override void OnInit()
        {
        }

        private void Start()
        {
            map = new Map(width, height, cellSize, new Vector3(-width / 2f, 0f, -height / 2f) * cellSize);
            map.SetGridParent(gridParent);
            map.Instantiate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                type = MapObjectType.Empty;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                type = MapObjectType.Road;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                type = MapObjectType.Wall;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                type = MapObjectType.SpawnPoint;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                type = MapObjectType.Destination;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Utils.GetMousePosition();
                map.SetGridType(pos, type);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (map.SetPath())
                {
                    Debug.Log("寻路成功");
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                Vector3 pos = Utils.GetMousePosition();
                Debug.Log(map.GetGridType(pos));
            }
        }
    }
}
