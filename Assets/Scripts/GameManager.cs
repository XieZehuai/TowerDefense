using System;
using UnityEngine;

namespace TowerDefense
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public enum GameState
        {
            Init,
            Playing,
            Paused,
            Over,
        }

        private GameState state;

        public GameState State => state;

        protected override void OnInit()
        {
        }
    }
}
