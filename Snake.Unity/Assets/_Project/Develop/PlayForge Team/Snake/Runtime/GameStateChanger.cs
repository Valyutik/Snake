using UnityEngine;

namespace PlayForge_Team.Snake.Runtime
{
    public sealed class GameStateChanger : MonoBehaviour
    {
        [SerializeField] private GameField gameField;
        [SerializeField] private Snakes.Snake snake;

        private void Start()
        {
            FirstStartGame();
        }

        private void FirstStartGame()
        {
            gameField.FillCellsPositions();
            snake.CreateSnake();
        }
    }
}