using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        private PathFinder finder;
        private int width;
        private int height;
        private MapObject[,] datas;
        private Vector2Int end;
        private Vector2Int[] startPosArray;
        private List<Vector2Int>[] paths;

        private float timer = 0f;

        private void Start()
        {
            finder = new PathFinder();
            width = 5;
            height = 5;
            end = new Vector2Int(width - 1, height - 1);

            datas = new MapObject[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    datas[i, j] = new MapObject(MapObjectType.Road, i, j);
                }
            }
            datas[1, 1].type = MapObjectType.Wall;
            datas[1, 2].type = MapObjectType.Wall;
            datas[1, 3].type = MapObjectType.Wall;
            datas[1, 4].type = MapObjectType.Wall;
            datas[1, 0].type = MapObjectType.Wall;

            int count = 5;
            startPosArray = new Vector2Int[count];
            paths = new List<Vector2Int>[count];

            for (int i = 0; i < count; i++)
            {
                startPosArray[i] = new Vector2Int(0, i);
                paths[i] = new List<Vector2Int>();
            }

            finder.SetMapData(width, height, datas, end);
            if (finder.FindPaths(startPosArray, true, ref paths))
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    Debug.Log("");
                    foreach (var item in paths[i])
                    {
                        Debug.Log(item);
                    }
                }
            }
            else
            {
                Debug.Log("寻路失败");
            }
        }

        private void Update()
        {
            return;

            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0f;
                finder.SetMapData(width, height, datas, end);
                finder.FindPaths(startPosArray, true, ref paths);
            }
        }
    }
}
