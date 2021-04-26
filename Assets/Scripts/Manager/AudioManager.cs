using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class AudioManager : Singleton<AudioManager>
	{
		private readonly Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();

		public void Play(string clipName, float duration, Vector3 pos)
        {
			if (!audioClipDic.ContainsKey(clipName))
            {
				AudioClip clip = ResourceManager.Load<AudioClip>(clipName);

				if (clip == null)
                {
					Debug.LogError("找不到音效文件：" + clipName);
					return;
                }

				audioClipDic.Add(clipName, clip);
            }

			ObjectPool.Spawn<AudioPlayer>(Res.AudioPlayerPrefab, pos).Play(audioClipDic[clipName], duration);
		}
	}
}
