using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();

        [SerializeField] private AudioSource bgmPlayer;

        public bool IsAudioOn { get; private set; }

        public bool IsMusicOn { get; private set; }

        public float AudioVolume { get; private set; }

        public float MusicVolume { get; private set; }

        protected override void OnInit()
        {
            IsAudioOn = true;
            IsMusicOn = true;
            AudioVolume = 1f;
        }

        public void TurnOnAudio()
        {
            IsAudioOn = true;
        }

        public void TurnOffAudio()
        {
            IsAudioOn = false;
            ObjectPool.UnspawnAll(Res.AudioPlayerPrefab);
        }

        public void TurnOnMusic()
        {
            IsMusicOn = true;

            if (!bgmPlayer.isPlaying)
            {
                bgmPlayer.Play();
            }
        }

        public void TurnOffMusic()
        {
            IsMusicOn = false;

            if (bgmPlayer.isPlaying)
            {
                bgmPlayer.Stop();
            }
        }

        public void SetAudioVolume(float volume)
        {
            AudioVolume = Mathf.Clamp01(volume);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            bgmPlayer.volume = AudioVolume;
        }

        public void Play(Vector3 pos, string clipName)
        {
            if (!IsAudioOn) return;

            if (HasAudioClip(clipName))
            {
                Play(pos, clipName, AudioVolume, audioClipDic[clipName].length, true, false);
            }
        }

        public void Play(Vector3 pos, string clipName, float duration)
        {
            if (!IsAudioOn) return;

            if (HasAudioClip(clipName))
            {
                Play(pos, clipName, AudioVolume, duration, true, false);
            }
        }

        private void Play(Vector3 pos, string clipName, float volume, float duration, bool replay, bool loop)
        {
            ObjectPool.Spawn<AudioPlayer>(Res.AudioPlayerPrefab, pos).Play(audioClipDic[clipName], volume, duration, replay, loop);
        }

        private bool HasAudioClip(string clipName)
        {
            if (!audioClipDic.ContainsKey(clipName))
            {
                AudioClip clip = ResourceManager.Load<AudioClip>(clipName);

                if (clip == null)
                {
                    Debug.LogError("找不到音效文件：" + clipName);
                    return false;
                }

                audioClipDic.Add(clipName, clip);
            }

            return true;
        }
    }
}
