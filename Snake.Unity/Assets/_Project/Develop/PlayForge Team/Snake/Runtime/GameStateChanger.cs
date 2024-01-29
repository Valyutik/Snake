using PlayForge_Team.Snake.Runtime.Apples;
using PlayForge_Team.Snake.Runtime.UI;
using UnityEngine;
using TMPro;

namespace PlayForge_Team.Snake.Runtime
{
    public sealed class GameStateChanger : MonoBehaviour
    {
        [SerializeField] private Score score;
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject gameEndScreen;
        [SerializeField] private TextMeshProUGUI gameEndScoreText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private AppleSpawner[] appleSpawners;
        [SerializeField] private GameField gameField;
        [SerializeField] private Snakes.Snake snake;
        private bool _isGameStarted;

        private void Start()
        {
            FirstStartGame();
        }
        
        public void RestartGame()
        {
            _isGameStarted = true;
            snake.RestartGame();

            foreach (var t in appleSpawners)
            {
                t.Restart();
            }
            score.Restart();
            SwitchScreens(true);
        }

        public void EndGame()
        {
            if (!_isGameStarted)
            {
                return;
            }
            _isGameStarted = false;

            snake.StopGame();
            RefreshScores();
            SwitchScreens(false);
        }

        private void StartGame()
        {
            _isGameStarted = true;
            snake.StartGame();
            
            foreach (var t in appleSpawners)
            {
                t.CreateApple();
            }
            SwitchScreens(true);
        }

        private void FirstStartGame()
        {
            gameField.FillCellsPositions();
            StartGame();
        }

        private void SwitchScreens(bool isGame)
        {
            gameScreen.SetActive(isGame);
            gameEndScreen.SetActive(!isGame);
        }

        private void RefreshScores()
        {
            var value = score.GetScore();
            var oldBestScore = score.GetBestScore();
            var isNewBestScore = CheckNewBestScore(value, oldBestScore);
            SetActiveGameEndScoreText(!isNewBestScore);
            if (isNewBestScore)
            {
                score.SetBestScore(value);
                SetNewBestScoreText(value);
            }
            else
            {
                SetGameEndScoreText(value);
                SetOldBestScoreText(oldBestScore);
            }
        }
        
        private bool CheckNewBestScore(int value, int oldBestScore)
        {
            return value > oldBestScore;
        }

        private void SetGameEndScoreText(int value)
        {
            gameEndScoreText.text = $"Игра окончена!\nКоличество очков: {value}";
        }

        private void SetOldBestScoreText(int value)
        {
            bestScoreText.text = $"Лучший результат: {value}";
        }

        private void SetNewBestScoreText(int value)
        {
            bestScoreText.text = $"Новый рекорд: {value}!";
        }

        private void SetActiveGameEndScoreText(bool value)
        {
            gameEndScoreText.gameObject.SetActive(value);
        }
    }
}