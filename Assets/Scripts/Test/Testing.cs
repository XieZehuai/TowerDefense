using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        private PathFinder finder;
        private NativeArray<int2> starts;
        private NativeList<int2>[] paths;
        private float timer = 0f;
        private int width;
        private int height;
        private MapObject[,] datas;
        private int2 end;

        private void Start()
        {
            finder = new PathFinder();
            width = 100;
            height = 100;
            end = new int2(width - 1, height - 1);

            datas = new MapObject[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    datas[i, j] = new MapObject(MapObjectType.Road, i, j);
                }
            }

            starts = new NativeArray<int2>(50, Allocator.Persistent);
            for (int i = 0; i < starts.Length; i++)
            {
                starts[i] = new int2(0, 0);
            }

            paths = new NativeList<int2>[starts.Length];
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                Debug.Log("");
                timer = 0f;
                finder.Init(width, height, datas, end);
                finder.FindPaths(starts, ref paths);
            }
        }
    }
}
