using System;
using UnityEngine;

namespace TowerDefense
{
    public class InputManager : SubStageManager
    {
        private MapObjectType selectedType = MapObjectType.Wall;

        private readonly KeyCode emptyKey = KeyCode.Alpha1;
        private readonly KeyCode roadKey = KeyCode.Alpha2;
        private readonly KeyCode wallKey = KeyCode.Alpha3;
        private readonly KeyCode spawnPointKey = KeyCode.Alpha4;
        private readonly KeyCode destinationKey = KeyCode.Alpha5;

        public InputManager(StageManager stageManager) : base(stageManager)
        {
        }

        public override void OnUpdate()
        {
            if (manager.IsPreparing || manager.IsPlaying)
            {
                if (manager.IsPreparing)
                {
                    StartGame();
                }

                ChangeSelectedType();
                ChangeMap();

                if (manager.IsPlaying)
                {
                    Pause();
                    Replay();
                }
            }
            else if (manager.IsPaused)
            {
                Continue();
            }
        }

        private void StartGame()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                manager.StartGame();
            }
        }

        private void Pause()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                manager.Pause();
            }
        }

        private void Continue()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                manager.Continue();
            }
        }

        private void ChangeSelectedType()
        {
            if (Input.GetKeyDown(emptyKey))
            {
                selectedType = MapObjectType.Empty;
            }
            else if (Input.GetKeyDown(roadKey))
            {
                selectedType = MapObjectType.Road;
            }
            else if (Input.GetKeyDown(wallKey))
            {
                selectedType = MapObjectType.Wall;
            }
            else if (Input.GetKeyDown(spawnPointKey))
            {
                selectedType = MapObjectType.SpawnPoint;
            }
            else if (Input.GetKeyDown(destinationKey))
            {
                selectedType = MapObjectType.Destination;
            }
        }

        private void ChangeMap()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Utils.GetMousePosition();
                manager.MapManager.ChangeGridType(pos, selectedType);
            }

            if (Input.GetMouseButtonDown(1))
            {
                manager.TowerManager.CreateTower(Utils.GetMousePosition());
            }

            if (Input.GetMouseButtonDown(2))
            {
                manager.TowerManager.RemoveTower(Utils.GetMousePosition());
            }
        }

        private void Replay()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                manager.Replay();
            }
        }
    }
}
