using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    public enum GameState
    {
        Idle,
        Playing,
        Paused,
        Over,
    }


    public class GameManager : MonoSingleton<GameManager>
    {
        private GameState state;
        public GameState State => state;

        public bool IsIdle => State == GameState.Idle;
        public bool IsPlaying => State == GameState.Playing;
        public bool IsPaused => State == GameState.Paused;
        public bool IsOver => State == GameState.Over;

        protected override void OnInit()
        {
            state = GameState.Idle;
            UIManager.Instance.Open<UIMainScene>();
        }

        public void LoadGameScene()
        {
            state = GameState.Playing;
            UIManager.Instance.Close<UIMainScene>();
            SceneManager.LoadScene("GameScene");
        }

        public void LoadMainScene()
        {
            state = GameState.Idle;
            UIManager.Instance.Open<UIMainScene>();
            SceneManager.LoadScene("MainScene");
        }

        public void GameOver()
        {
            state = GameState.Over;
        }
    }
}