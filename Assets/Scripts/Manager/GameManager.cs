using System;
using UnityEngine;

namespace TowerDefense
{
    public enum GameState
    {
        Init,
        Playing,
        Paused,
        Over,
    }


    public class GameManager : MonoSingleton<GameManager>
    {
        public MapObjectType type = MapObjectType.Empty;

        [Header("地图大小")] 
        [SerializeField] private Vector2Int mapSize = Vector2Int.one * 10;
        [SerializeField] private int cellSize = 4;

        [SerializeField] private float spawnInterval = 5f;

        [SerializeField] private DamageConfig damageConfig;

        private bool spawn;
        private GameState state = GameState.Init;

        public GameState State => state;

        protected override void OnInit()
        {
            EnemyManager.Instance.SetLevelData(spawnInterval);
            MapManager.Instance.CreateMap(mapSize.x, mapSize.y, cellSize);
            //EnemyManager.Instance.SetPath(MapManager.Instance.GetPaths());
        }

        private void FixedUpdate()
        {
            ChangeMap();

            if (Input.GetKeyDown(KeyCode.S))
            {
                spawn = true;
                EnemyManager.Instance.SetLevelData(spawnInterval);
            }

            if (spawn) EnemyManager.Instance.OnUpdate();
            Physics.SyncTransforms();
            TowerManager.Instance.OnUpdate();
        }

        public float GetDamage(float damage, AttackType attackType, ArmorType armorType)
        {
            return damageConfig.GetDamage(damage, attackType, armorType);
        }

        private void ChangeMap()
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
                MapManager.Instance.ChangeGridType(pos, type);
            }

            if (Input.GetMouseButtonDown(1))
            {
                TowerManager.Instance.CreateTower(Utils.GetMousePosition());
            }

            if (Input.GetMouseButtonDown(2))
            {
                TowerManager.Instance.RemoveTower(Utils.GetMousePosition());
            }
        }
    }
}