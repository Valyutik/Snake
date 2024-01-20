using UnityEngine;

namespace PlayForge_Team.Snake.Runtime.Apples
{
    public sealed class AppleSpawner : MonoBehaviour
    {
        [SerializeField] private int stepsBeforeSpawn;
        [SerializeField] private GameStateChanger gameStateChanger;
        [SerializeField] private GameField gameField;
        [SerializeField] private Snakes.Snake snake;
        [SerializeField] private GameFieldObject applePrefab;
        private GameFieldObject _apple;
        private Animation _appleAnimation;
        private int _stepCounter = -1;
        
        public void CreateApple()
        {
            _apple = Instantiate(applePrefab);

            _appleAnimation = _apple.GetComponent<Animation>();
            SetNextApple();
        }
        
        public void Restart()
        {
            _stepCounter = -1;
            SetNextApple();
        }
        
        public void SetNextApple()
        {
            if (!_apple)
            {
                return;
            }

            if (!CheckHasEmptyCells())
            {
                gameStateChanger.EndGame();
                return;
            }
            _stepCounter++;
            
            if(_stepCounter < stepsBeforeSpawn)
            {
                HideApple();
                return;
            }
            ShowApple();
            PlayAppleAnimation();

            var emptyCellsCount = GetEmptyCellsCount();
            var possibleCellsIds = new Vector2Int[emptyCellsCount];

            var counter = 0;
            for (var i = 0; i < gameField.CellsInRow; i++)
            {
                for (var j = 0; j < gameField.CellsInRow; j++)
                {
                    if (!gameField.GetCellIsEmpty(i, j)) continue;
                    possibleCellsIds[counter] = new Vector2Int(i, j); 
                    counter++;
                }
            }

            var appleCellId = possibleCellsIds[Random.Range(0, possibleCellsIds.Length)];
            gameField.SetObjectCell(_apple, appleCellId);
        }
        
        public Vector2Int GetAppleCellId()
        {
            return _apple.GetCellId();
        }
        
        public void HideApple()
        {
            SetActiveApple(false);
        }
        
        private void PlayAppleAnimation()
        {
            _appleAnimation.Play();
        }
        
        private void SetActiveApple(bool value)
        {
            _apple.gameObject.SetActive(value);
        }

        private void ShowApple()
        {
            _stepCounter = 0;
            SetActiveApple(true);
        }

        private bool CheckHasEmptyCells()
        {
            return GetEmptyCellsCount() > 0;
        }

        private int GetEmptyCellsCount()
        {
            var snakePartsLength = snake.GetSnakePartsLength();
            var fieldCellsCount = gameField.CellsInRow * gameField.CellsInRow;
            return fieldCellsCount - snakePartsLength;
        }
    }
}