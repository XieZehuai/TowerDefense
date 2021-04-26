using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public enum AudioType
    {
		ShellExplode,
    }


	public class AudioManager : MonoSingleton<AudioManager>
	{
		private readonly Dictionary<AudioType, AudioClip> audioClipDic = new Dictionary<AudioType, AudioClip>();

		public void Play(AudioType audioType, float duration, Vector3 pos)
        {
			if (!audioClipDic.ContainsKey(audioType))
            {
				//string clipName = audioType.ToString() + "Audio";
				//AudioClip clip = ResourceManager.Load<AudioClip>(clipName);
				AudioClip clip = ResourceManager.Load<AudioClip>(Res.ShellExplodeAudio);

				if (clip == null)
                {
					//Debug.LogError("找不到音效文件：" + clipName);
					Debug.LogError("找不到音效文件：" + Res.ShellExplodeAudio);
					return;
                }

				audioClipDic.Add(audioType, clip);
            }

			//ObjectPool.Spawn<AudioPlayer>("AudioPlayer", pos).Play(audioClipDic[audioType], duration);
			ObjectPool.Spawn<AudioPlayer>(Res.AudioPlayerPrefab, pos).Play(audioClipDic[audioType], duration);
		}
	}
}
