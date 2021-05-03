using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 在关卡场景中管理玩家的所有操作
    /// </summary>
    public class InputManager : SubStageManager
    {
        /// <summary>
        /// 当前选择的格子类型，修改地图时使用
        /// </summary>
        public MapObjectType GridType { get; set; } = MapObjectType.Wall;

        /// <summary>
        /// 当前选择的炮塔的ID，摆放炮塔时使用
        /// </summary>
        public int TowerId { get; set; } = -1;

        private float undoTimer; // 连续撤销操作的计时器

        public InputManager(StageManager stageManager) : base(stageManager)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            ControlCamera(deltaTime); // 控制相机运动

            if (manager.IsPreparing || manager.IsPlaying)
            {
                if (!UIManager.Instance.IsMouseOverUI)
                {
                    ChangeMap();
                    PlaceTower();
                }

                UndoChangeMap(deltaTime);
            }
        }

        #region 玩家鼠标控制事件，通过鼠标点击地图触发

        // 修改地图
        private void ChangeMap()
        {
            if (Input.GetMouseButton(0)) // 点击鼠标左键修改地图元素
            {
                Vector3 pos = Utils.GetMousePosition();
                manager.MapManager.ChangeGridType(pos, GridType, true);
            }
        }

        // 撤销对地图的修改
        private void UndoChangeMap(float deltaTime)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Z)) // 同时按住左Ctrl和Z键
            {
                if (Input.GetKeyDown(KeyCode.Z)) // 刚按下Z键
                {
                    manager.MapManager.Undo();
                    undoTimer = 0f;
                }
                else // 按下Z键并持续了一段时间
                {
                    if (undoTimer < Utils.UNDO_PRESS_DURATION)
                    {
                        undoTimer += deltaTime;
                    }
                    else
                    {
                        manager.MapManager.Undo();
                    }
                }
            }
        }

        // 摆放炮塔
        private void PlaceTower()
        {
            if (Input.GetMouseButtonDown(1) && TowerId != -1) // 点击鼠标右键摆放炮塔
            {
                manager.TowerManager.PlaceTower(Utils.GetMousePosition(), TowerId);
                TowerId = -1;
            }
        }

        // 控制相机运动
        private void ControlCamera(float deltaTime)
        {
            CameraController.Instance.Zoom(Input.GetAxis("Mouse ScrollWheel"));

            if (Input.GetMouseButton(2)) // 按住鼠标中键并拖动鼠标旋转相机
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                CameraController.Instance.Rotate(deltaTime, x, y);
            }

            // 用WASD控制相机移动
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 movement = new Vector2(h, v).normalized;
            CameraController.Instance.Move(deltaTime, movement);
        }

        #endregion
    }
}