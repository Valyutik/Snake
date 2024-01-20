using PlayForge_Team.Snake.Runtime.Apples;
using UnityEngine;

namespace PlayForge_Team.Snake.Runtime
{
    public sealed class GameStateChanger : MonoBehaviour
    {
        [SerializeField] private AppleSpawner appleSpawner;
        [SerializeField] private GameField gameField;
        [SerializeField] private Snakes.Snake snake;

        private void Start()
        {
            FirstStartGame();
        }

        public void EndGame()
        {
            snake.StopGame();
        }
        
        private void StartGame()
        {
            snake.StartGame();
            appleSpawner.CreateApple();
        }

        private void FirstStartGame()
        {
            gameField.FillCellsPositions();
            StartGame();
        }
    }
}