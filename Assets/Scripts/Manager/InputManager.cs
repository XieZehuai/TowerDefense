using System;
using UnityEngine;

namespace TowerDefense
{
    public class InputManager : SubStageManager
    {
        private MapObjectType selectedType = MapObjectType.Wall;

        public InputManager(StageManager stageManager) : base(stageManager)
        {
            TypeEventSystem.Register<StartGame>(StartGame);
            TypeEventSystem.Register<PauseGame>(Pause);
            TypeEventSystem.Register<ContinueGame>(Continue);
            TypeEventSystem.Register<ReplayGame>(Replay);
            TypeEventSystem.Register<SaveMap>(SaveMap);
            TypeEventSystem.Register<ChangeGridType>(ChangeGridType);
        }

        public override void OnUpdate()
        {
            if (manager.IsPreparing || manager.IsPlaying)
            {
                ChangeMap();
                PlaceTower();
                RemoveTower();
            }
        }

        #region 玩家点击事件，通过鼠标点击地图触发
        private void ChangeMap()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Utils.GetMousePosition();
                manager.MapManager.ChangeGridType(pos, selectedType);
            }
        }

        private void PlaceTower()
        {
            if (Input.GetMouseButtonDown(1))
            {
                manager.TowerManager.CreateTower(Utils.GetMousePosition());
            }
        }

        private void RemoveTower()
        {
            if (Input.GetMouseButtonDown(2))
            {
                manager.TowerManager.RemoveTower(Utils.GetMousePosition());
            }
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
            selectedType = context.type;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            TypeEventSystem.UnRegister<StartGame>(StartGame);
            TypeEventSystem.UnRegister<PauseGame>(Pause);
            TypeEventSystem.UnRegister<ContinueGame>(Continue);
            TypeEventSystem.UnRegister<ReplayGame>(Replay);
            TypeEventSystem.UnRegister<SaveMap>(SaveMap);
            TypeEventSystem.UnRegister<ChangeGridType>(ChangeGridType);
        }
    }
}
