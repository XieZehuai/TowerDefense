using System.IO;
using UnityEngine;

namespace TowerDefense
{
	#region 地图格子的类型

	/// <summary>
	/// 地图格子的类型
	/// </summary>
	public enum MapObjectType
	{
		Empty = 1, // 空格子
		Road = 2, // 敌人行走的道路
		Wall = 3, // 可以摆放炮塔且敌人不能通过的墙壁
		SpawnPoint = 4, // 敌人的出生点
		Destination = 5, // 敌人的目标点
		WallWithTower = 6, // 摆放了炮塔的墙壁

		None = 0, // 非格子的其他地方
	}

	#endregion


	#region 地图上的格子

	/// <summary>
	/// 地图上的格子
	/// </summary>
	public class MapObject
	{
		public MapObjectType type;
		public int x;
		public int y;

		public MapObject(MapObjectType type, int x, int y)
		{
			this.type = type;
			this.x = x;
			this.y = y;
		}

		public bool IsWalkable()
		{
			return type == MapObjectType.Road || type == MapObjectType.Destination || type == MapObjectType.SpawnPoint;
		}

		public override int GetHashCode()
		{
			return x << 16 + y;
		}
	}

	#endregion


	public class Map : Grid<MapObject>
	{
		#region 构造函数

		public Map(int width, int height, int cellSize, Vector3 originPos) : base(width, height, cellSize, originPos)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					gridArray[x, y] = new MapObject(MapObjectType.Road, x, y);
				}
			}

			gridArray[width - 1, height - 1].type = MapObjectType.Destination;
			gridArray[0, 0].type = MapObjectType.SpawnPoint;
		}

		public Map(int width, int height, int cellSize) : this(width, height, cellSize, Vector3.zero)
		{
		}

		public Map(MapObject[,] objects, int cellSize, Vector3 originPos) : base(objects, cellSize, originPos)
		{
		}

		public Map(MapObject[,] objects, int cellSize) : this(objects, cellSize, Vector3.zero)
		{
		}

		#endregion

		public void SetGridType(int x, int y, MapObjectType type)
		{
			gridArray[x, y].type = type;
		}

		public void SetGridType(Vector3 worldPosition, MapObjectType type)
		{
			if (GetGridPosition(worldPosition, out int x, out int y))
			{
				SetGridType(x, y, type);
			}
		}

		public override void SetValue(int x, int y, MapObject value)
		{
			SetGridType(x, y, value.type);
		}

		public override void SetValue(Vector3 worldPosition, MapObject value)
		{
			if (GetGridPosition(worldPosition, out int x, out int y))
			{
				SetGridType(x, y, value.type);
			}
		}

		public MapObjectType GetGridType(int x, int y)
		{
			if (x >= 0 && x < width && y >= 0 && y < height)
			{
				return GetValue(x, y).type;
			}

			return MapObjectType.None;
		}

		public MapObjectType GetGridType(Vector3 worldPosition)
		{
			if (GetGridPosition(worldPosition, out int x, out int y))
			{
				return GetGridType(x, y);
			}

			return MapObjectType.None;
		}

		public void Save(string path)
		{
			using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
			{
				writer.Write(width);
				writer.Write(height);
				writer.Write(cellSize);

				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (gridArray[x, y].type == MapObjectType.WallWithTower)
						{
							writer.Write((int)MapObjectType.Wall);
						}
						else
						{
							writer.Write((int)gridArray[x, y].type);
						}
					}
				}
			}
		}
	}
}