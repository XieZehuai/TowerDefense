using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    //public enum GameState
    //{
    //    Idle,
    //    Playing,
    //    Paused,
    //    Over,
    //}


    public class GameManager : MonoSingleton<GameManager>
    {
        protected override void OnInit()
        {
            UIManager.Instance.Open<UIMainScene>();
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.Close<UIMainScene>();
        }

        public void LoadMainScene()
        {
            SceneManager.LoadScene("MainScene");
            UIManager.Instance.Open<UIMainScene>();
        }
    }
}
