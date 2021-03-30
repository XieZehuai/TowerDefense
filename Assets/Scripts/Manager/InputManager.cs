using UnityEngine;

namespace TowerDefense
{
    public class InputManager : SubStageManager
    {
        private MapObjectType selectedGridType = MapObjectType.Wall;
        private int selectedTowerId = 0;
        private const float undoDuration = 0.5f;
        private float undoTimer;

        public InputManager(StageManager stageManager) : base(stageManager)
        {
            TypeEventSystem.Register<StartGame>(StartGame);
            TypeEventSystem.Register<PauseGame>(Pause);
            TypeEventSystem.Register<ContinueGame>(Continue);
            TypeEventSystem.Register<ReplayGame>(Replay);
            TypeEventSystem.Register<SaveMap>(SaveMap);
            TypeEventSystem.Register<ChangeGridType>(ChangeGridType);
            TypeEventSystem.Register<ChangeTowerType>(ChangeTowerType);
            TypeEventSystem.Register<TogglePathIndicator>(TogglePathIndicator);
        }

        public override void OnUpdate(float deltaTime)
        {
            ControlCamera(deltaTime);

            if (manager.IsPreparing || manager.IsPlaying)
            {
                if (!UIManager.Instance.IsMouseOverUI)
                {
                    ChangeMap();
                    PlaceTower();
                    RemoveTower();
                }

                UndoChangeMap();
            }

            // if (!UIManager.Instance.IsMouseOverUI && (manager.IsPreparing || manager.IsPlaying))
            // {
            //     ChangeMap();
            //     PlaceTower();
            //     RemoveTower();
            // }

            // if (Input.GetKeyDown(KeyCode.G))
            // {
            //     manager.SpeedUp(true);
            // }
            // else if (Input.GetKeyDown(KeyCode.H))
            // {
            //     manager.SpeedUp(false);
            // }
        }

        #region 玩家鼠标控制事件，通过鼠标点击地图触发
        private void ChangeMap()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Utils.GetMousePosition();
                manager.MapManager.ChangeGridType(pos, selectedGridType, true);
            }
        }

        private void UndoChangeMap()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Z))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    manager.MapManager.Undo();
                    undoTimer = 0f;
                }
                else
                {
                    if (undoTimer < undoDuration)
                    {
                        undoTimer += Time.deltaTime;
                    }
                    else
                    {
                        manager.MapManager.Undo();
                    }
                }
            }

            // if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            // {
            //     manager.MapManager.Undo();

            //     if (Input.GetKey(KeyCode.Z))
            //     {
            //         if (undoTimer < undoDuration)
            //         {
            //             undoTimer += Time.deltaTime;
            //         }
            //         else
            //         {

            //         }
            //     }
            // }
        }

        private void PlaceTower()
        {
            if (Input.GetMouseButtonDown(1))
            {
                manager.TowerManager.CreateTower(Utils.GetMousePosition(), selectedTowerId);
            }
        }

        private void RemoveTower()
        {
            if (Input.GetMouseButtonDown(2))
            {
                manager.TowerManager.RemoveTower(Utils.GetMousePosition());
            }
        }

        private void ControlCamera(float deltaTime)
        {
            manager.CameraController.Zoom(Input.GetAxis("Mouse ScrollWheel"));

            if (Input.GetMouseButton(2))
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                manager.CameraController.Rotate(x, y, deltaTime);
            }

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 movement = new Vector2(h, v).normalized;
            manager.CameraController.Move(movement, deltaTime);
        }
        #endregion

        #region UI输入事件，全部通过发送消息触发
        private void StartGame(StartGame context)
        {
            manager.StartGame();
        }

        private void Pause(PauseGame context)
        {
            manager.Pause();
        }

        private void Continue(ContinueGame context)
        {
            manager.Continue();
        }

        private void Replay(ReplayGame context)
        {
            manager.Replay();
        }

        private void SaveMap(SaveMap context)
        {
            manager.SaveMapData();
        }

        private void ChangeGridType(ChangeGridType context)
        {
            selectedGridType = context.type;
        }

        private void ChangeTowerType(ChangeTowerType context)
        {
            selectedTowerId = context.towerId;
        }

        private void TogglePathIndicator(TogglePathIndicator context)
        {
            manager.PathIndicator.TogglePathIndicator();
        }
        #endregion

        protected override void OnDispose()
        {
            TypeEventSystem.UnRegister<StartGame>(StartGame);
            TypeEventSystem.UnRegister<PauseGame>(Pause);
            TypeEventSystem.UnRegister<ContinueGame>(Continue);
            TypeEventSystem.UnRegister<ReplayGame>(Replay);
            TypeEventSystem.UnRegister<SaveMap>(SaveMap);
            TypeEventSystem.UnRegister<ChangeGridType>(ChangeGridType);
            TypeEventSystem.UnRegister<ChangeTowerType>(ChangeTowerType);
            TypeEventSystem.UnRegister<TogglePathIndicator>(TogglePathIndicator);
        }
    }
}