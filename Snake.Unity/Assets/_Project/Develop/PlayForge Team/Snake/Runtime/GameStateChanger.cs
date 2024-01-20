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
        [SerializeField] private AppleSpawner[] AppleSpawners;
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

            foreach (var t in AppleSpawners)
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
            
            foreach (var t in AppleSpawners)
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
            var score = this.score.GetScore();
            var oldBestScore = this.score.GetBestScore();
            var isNewBestScore = CheckNewBestScore(score, oldBestScore);
            SetActiveGameEndScoreText(!isNewBestScore);
            if (isNewBestScore)
            {
                this.score.SetBestScore(score);
                SetNewBestScoreText(score);
            }
            else
            {
                SetGameEndScoreText(score);
                SetOldBestScoreText(oldBestScore);
            }
        }
        
        private bool CheckNewBestScore(int score, int oldBestScore)
        {
            return score > oldBestScore;
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