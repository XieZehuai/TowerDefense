using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        [Header("是否测试寻路算法")]
        public bool testPathFindingStrategy;
        
        [SerializeField] private int mapWidth = 20;
        [SerializeField] private int mapHeight = 20;
        [SerializeField] private int startPointCount = 5;
        [SerializeField] private Vector2Int startPoint;
        [SerializeField] private Vector2Int endPoint;

        private PathFinder dijkstra = new PathFinder(PathFindingStrategy.Dijkstra);
        private PathFinder flowField = new PathFinder(PathFindingStrategy.FlowField);
        private PathFinder dots = new PathFinder(PathFindingStrategy.DOTS);
        private PathFinder aStar = new PathFinder(PathFindingStrategy.AStar);

        private float timer;
        private MapObject[,] mapDatas;
        private Vector2Int[] startPoints;
        private List<Vector2Int>[] paths;

        private void Start()
        {
            mapDatas = new MapObject[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    mapDatas[x, y] = new MapObject(MapObjectType.Road, x, y);
                }
            }
            //SetWall();

            startPoints = new Vector2Int[startPointCount];
            for (int i = 0; i < startPointCount; i++)
            {
                startPoints[i] = startPoint;
            }

            paths = new List<Vector2Int>[startPointCount];
        }

        private void Update()
        {
            if (!testPathFindingStrategy) return;
            
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0f;

                Debug.Log("================================");
                dijkstra.SetMapData(mapDatas);
                flowField.SetMapData(mapDatas);
                dots.SetMapData(mapDatas);
                aStar.SetMapData(mapDatas);

                dijkstra.FindPaths(startPoints, endPoint, ref paths, true);
                flowField.FindPaths(startPoints, endPoint, ref paths, true);
                dots.FindPaths(startPoints, endPoint, ref paths, true);
                aStar.FindPaths(startPoints, endPoint, ref paths, true);
            }
        }

        private void SetWall()
        {
            for (int i = 0; i < mapWidth - 1; i++)
            {
                mapDatas[i, 1].type = MapObjectType.Wall;
                mapDatas[i, 5].type = MapObjectType.Wall;
                mapDatas[i, 9].type = MapObjectType.Wall;
                mapDatas[i, 13].type = MapObjectType.Wall;
                mapDatas[i, 17].type = MapObjectType.Wall;
            }
            for (int i = 1; i < mapWidth; i++)
            {
                mapDatas[i, 3].type = MapObjectType.Wall;
                mapDatas[i, 7].type = MapObjectType.Wall;
                mapDatas[i, 11].type = MapObjectType.Wall;
                mapDatas[i, 15].type = MapObjectType.Wall;
            }
        }
    }
}
