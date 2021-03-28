using UnityEngine;
using UnityEngine.VFX;

namespace TowerDefense
{
    public class Particle : PoolObject
    {
        private VisualEffect vfx;

        private bool isFollowing = false;
        private Transform followTarget = null;
        private Vector3 offset;

        public bool IsPlaying { get; private set; }

        private void Awake()
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

        public void Play()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;
            vfx.Play();
        }

        public void Replay()
        {
            IsPlaying = true;
            vfx.Play();
        }

        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }

            vfx.Stop();
        }

        public void SetFloat(string fieldName, float value)
        {
            vfx.SetFloat(fieldName, value);
        }

        public Particle Follow(Transform followTarget, Vector3 offset)
        {
            isFollowing = true;
            this.followTarget = followTarget;
            this.offset = offset;

            return this;
        }

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
