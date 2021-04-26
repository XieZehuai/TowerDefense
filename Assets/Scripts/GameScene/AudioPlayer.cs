using UnityEngine;

namespace TowerDefense
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : PoolObject
    {
        private bool isPlaying;
        private AudioSource audioSource;

        protected override void OnInstantiate()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, float volume, float duration = -1f, bool replay = false, bool loop = false)
        {
            if (isPlaying && !replay) return;

            isPlaying = true;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.Play();

            if (duration > 0f)
            {
                this.Invoke(Stop, duration);
            }
        }

        public void Stop()
        {
            if (!isPlaying) return;

            isPlaying = false;
            audioSource.Stop();
            UnspawnSelf();
        }
    }
}
