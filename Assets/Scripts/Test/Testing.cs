using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        public bool test;
        
        [SerializeField] private int mapWidth = 20;
        [SerializeField] private int mapHeight = 20;
        [SerializeField] private int startPointCount = 5;
        [SerializeField] private Vector2Int startPoint;
        [SerializeField] private Vector2Int endPoint;

        private PathFinder dijkstra = new PathFinder(new DijkstraPathFinding());
        private PathFinder reverseDijkstra = new PathFinder(new ReverseDijkstraPathFinding());
        private PathFinder dots = new PathFinder(new DOTSPathFinding());
        private PathFinder aStar = new PathFinder(new AStarPathFinding());

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

            startPoints = new Vector2Int[startPointCount];
            for (int i = 0; i < startPointCount; i++)
            {
                startPoints[i] = startPoint;
            }

            paths = new List<Vector2Int>[startPointCount];
        }

        private void Update()
        {
            if (!test) return;
            
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0f;

                Debug.Log("================================");
                dijkstra.SetMapData(mapDatas);
                reverseDijkstra.SetMapData(mapDatas);
                dots.SetMapData(mapDatas);
                aStar.SetMapData(mapDatas);

                dijkstra.FindPaths(startPoints, endPoint, ref paths, true);
                reverseDijkstra.FindPaths(startPoints, endPoint, ref paths, true);
                dots.FindPaths(startPoints, endPoint, ref paths, true);
                aStar.FindPaths(startPoints, endPoint, ref paths, true);
            }
        }
    }
}
