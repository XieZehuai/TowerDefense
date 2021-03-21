using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 摄像机控制器
    /// </summary>
    public class CameraController : MonoSingleton<CameraController>
    {
        [Header("缩放控制参数")]
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField, Range(0.5f, 5f)] private float zoomSpeed = 3f;

        [Header("旋转控制参数")]
        [SerializeField] private float horizontalSpeed = 50f;
        [SerializeField] private float verticalSpeed = 30f;
        [SerializeField] private float maxAngle = 70f;
        [SerializeField] private float minAngle = -20f;

        [SerializeField] private float moveSpeed = 2f;

        [SerializeField] private Transform cameraContainer = default;

        private float distance = 15f;
        private Vector3 rotation;

        protected override void OnInit()
        {
            cameraContainer.localPosition = Vector3.back * distance;
            rotation = transform.localEulerAngles;
        }

        /// <summary>
        /// 相机移动
        /// </summary>
        /// <param name="movement">XZ平面上的位移</param>
        public void Move(Vector2 movement)
        {
            movement *= moveSpeed * Time.deltaTime;
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            transform.localPosition += right * movement.x + forward * movement.y;
        }

        /// <summary>
        /// 相机旋转
        /// </summary>
        /// <param name="x">水平方向上的旋转</param>
        /// <param name="y">垂直方向上的旋转</param>
        public void Rotate(float x, float y)
        {
            x *= horizontalSpeed;
            y *= verticalSpeed;
            rotation += new Vector3(-y, x, 0f) * Time.deltaTime;
            rotation.x = Mathf.Clamp(rotation.x, minAngle, maxAngle);
            transform.localRotation = Quaternion.Euler(rotation);
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="zoom">缩放值</param>
        public void Zoom(float zoom)
        {
            distance -= zoom * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            cameraContainer.localPosition = Vector3.back * distance;
        }
    }
}
