using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 格子类，用于构建网格系统
    /// </summary>
    /// <typeparam name="T">格子元素类型</typeparam>
    public class Grid<T>
    {
        private int width; // 网格宽度
        private int height; // 网格高度
        private int cellSize; // 每个格子的大小
        private T[,] gridArray; // 所有格子数据

        /// <summary>
        /// 网格的宽度
        /// </summary>
        public int Width => width;

        /// <summary>
        /// 网格的高度
        /// </summary>
        public int Height => height;

        /// <summary>
        /// 单元格大小
        /// </summary>
        public int CellSize => cellSize;

        /// <summary>
        /// 所有格子的数据
        /// </summary>
        public T[,] GridArray => gridArray;

        /// <summary>
        /// 创建一个网格
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="cellSize">单元格大小</param>
        public Grid(int width, int height, int cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            gridArray = new T[width, height];

            DrawGrid(Color.white);
        }

        /// <summary>
        /// 根据二维坐标设置格子的数据
        /// </summary>
        /// <param name="x">目标格子x轴坐标</param>
        /// <param name="y">目标格子y轴坐标</param>
        /// <param name="value">格子的数据</param>
        public void SetValue(int x, int y, T value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                Debug.Log("change " + x + ", " + y + " grid value");
                gridArray[x, y] = value;
            }
        }

        /// <summary>
        /// 根据世界坐标设置格子的数据
        /// </summary>
        /// <param name="worldPosition">目标格子的世界坐标</param>
        /// <param name="value">格子的数据</param>
        public void SetValue(Vector3 worldPosition, T value)
        {
            GetGridPosition(worldPosition, out int x, out int y);
            SetValue(x, y, value);
        }

        /// <summary>
        /// 根据二维坐标获取格子数据
        /// </summary>
        /// <param name="x">目标格子x轴坐标</param>
        /// <param name="y">目标格子y轴坐标</param>
        /// <param name="defaultValue">获取失败时返回的值</param>
        /// <returns>格子的数据，失败返回默认值</returns>
        public T GetValue(int x, int y, T defaultValue = default)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return gridArray[x, y];
            }

            return defaultValue;
        }

        /// <summary>
        /// 根据世界坐标获取格子数据
        /// </summary>
        /// <param name="worldPosition">格子的世界坐标</param>
        /// <param name="defaultValue">获取失败时返回的值</param>
        /// <returns>格子的数据，失败返回默认值</returns>
        public T GetValue(Vector3 worldPosition, T defaultValue = default)
        {
            GetGridPosition(worldPosition, out int x, out int y);
            return GetValue(x, y, defaultValue);
        }

        // 将二维坐标转为世界坐标
        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0f, y) * cellSize;
        }

        // 将世界坐标转为二维坐标
        private void GetGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / cellSize);
            y = Mathf.FloorToInt(worldPosition.z / cellSize);
        }

        // 画出网格图
        private void DrawGrid(Color color, float duration = 100f)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), color
                        , duration);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), color
                        , duration);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height),
                color, duration);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), color,
                duration);
        }
    }
}