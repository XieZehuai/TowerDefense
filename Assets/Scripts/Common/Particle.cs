using UnityEngine;
using UnityEngine.VFX;

namespace TowerDefense
{
    /// <summary>
    /// VFX粒子系统
    /// </summary>
    public class Particle : PoolObject
    {
        private VisualEffect vfx; // vfx对象

        private bool isFollowing = false; // 是否正在跟随目标移动
        private Transform followTarget = null; // 跟随的目标
        private Vector3 offset; // 跟随的位置偏移

        /// <summary>
        /// 是否正在播放特效
        /// </summary>
        public bool IsPlaying { get; private set; }

        protected override void OnInstantiate()
        {
            vfx = GetComponentInChildren<VisualEffect>();
            if (vfx == null)
            {
                Debug.LogError("没有对应的特效");
            }
        }

        private void Update()
        {
            if (isFollowing)
            {
                transform.localPosition = followTarget.localPosition + offset;
            }
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        public void Play()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;
            vfx.Play();
        }

        /// <summary>
        /// 重播特效
        /// </summary>
        public void Replay()
        {
            IsPlaying = true;
            vfx.Play();
        }

        /// <summary>
        /// 暂停播放特效
        /// </summary>
        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }

            vfx.Stop();
        }

        /// <summary>
        /// 设置float类型属性值
        /// </summary>
        /// <param name="fieldName">变量名</param>
        /// <param name="value">数值</param>
        public void SetFloat(string fieldName, float value)
        {
            vfx.SetFloat(fieldName, value);
        }

        /// <summary>
        /// 让特效跟随目标移动
        /// </summary>
        /// <param name="followTarget">目标对象</param>
        /// <param name="offset">偏移位置</param>
        public Particle Follow(Transform followTarget, Vector3 offset)
        {
            isFollowing = true;
            this.followTarget = followTarget;
            this.offset = offset;

            return this;
        }

        /// <summary>
        /// 取消跟随目标移动
        /// </summary>
        public void NotFollow()
        {
            isFollowing = false;
            followTarget = null;
            offset = Vector3.zero;
        }

        public override void OnUnspawn()
        {
            NotFollow();
        }

        public override void OnReclaim()
        {
            NotFollow();
        }
    }
}
