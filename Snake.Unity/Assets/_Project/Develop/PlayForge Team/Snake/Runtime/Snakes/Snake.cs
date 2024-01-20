using PlayForge_Team.Snake.Runtime.Apples;
using PlayForge_Team.Snake.Runtime.UI;
using UnityEngine;
using System;

namespace PlayForge_Team.Snake.Runtime.Snakes
{
    public sealed class Snake : MonoBehaviour
    {
        [SerializeField] private float ExplosionForce = 60;
        [SerializeField] private AppleSpawner bonusAppleSpawner;
        [SerializeField] private Score score;
        [SerializeField] private AppleSpawner appleSpawner;
        [SerializeField] private GameStateChanger gameStateChanger;
        [SerializeField] private GameField gameField;
        [SerializeField] private GameFieldObject headPrefab;
        [SerializeField] private GameFieldObject bodyPrefab;
        [SerializeField] private Vector2Int startCellId = new(5, 5);
        [SerializeField] private float moveDelay = 1.3f;
        private GameFieldObject[] _parts;
        private Vector2Int _moveDirection = Vector2Int.up;
        private float _moveTimer;
        private bool _isActive;
        
        private void Update()
        {
            if (!_isActive)
            {
                return;
            }
            GetMoveDirection();
            MoveTimerTick();
        }
        
        public void RestartGame()
        {
            DestroySnake();
            StartGame();
        }
        
        public void StartGame()
        {
            CreateSnake();
            _isActive = true;
        }

        public void StopGame()
        {
            _isActive = false;
        }
        
        public int GetSnakePartsLength()
        {
            return _parts.Length;
        }
        
        private void DestroySnake()
        {
            foreach (var t in _parts)
            {
                Destroy(t.gameObject);
            }
        }
        
        private void ExplodeSnake(Vector2 explodePosition)
        {
            foreach (var t in _parts)
            {
                var partRigid = t.GetComponent<Rigidbody2D>();
                partRigid.simulated = true;
                var partPosition = gameField.GetCellPosition(t.GetCellId());
                var explodeDirection = partPosition - explodePosition;
                partRigid.AddForce((explodeDirection.normalized + Vector2.up) * ExplosionForce, ForceMode2D.Impulse);
            }
        }

        private void CreateSnake()
        {
            _parts = Array.Empty<GameFieldObject>();
            AddPart(headPrefab, startCellId);
            AddPart(bodyPrefab, startCellId + Vector2Int.down);
        }

        private void CheckNextCellFail(Vector2Int nextCellId)
        {
            for (var i = 1; i < _parts.Length; i++)
            {
                if (_parts[i].GetCellId() != nextCellId) continue;
                gameStateChanger.EndGame();
                var partPosition = gameField.GetCellPosition(_parts[i].GetCellId());
                ExplodeSnake(partPosition);
            }
        }


        private void CheckNextCellApple(Vector2Int nextCellId, Vector2Int cellIdForAddPart)
        {
            if(appleSpawner.GetAppleCellId() == nextCellId)
            {
                AddPart(bodyPrefab, cellIdForAddPart);
                appleSpawner.SetNextApple();
                bonusAppleSpawner.SetNextApple();

                score.AddScore(1);
            }
            else if(bonusAppleSpawner.GetAppleCellId() == nextCellId)
            {
                const int countToRemove = 2;
                for (var i = 0; i < countToRemove; i++)
                {
                    RemovePart();
                }
                bonusAppleSpawner.HideApple();
            }
        }
        
        private void RemovePart()
        {
            Destroy(_parts[^1].gameObject);
            ChangePartsArrayLength(-1);
        }
        
        private void AddPart(GameFieldObject partPrefab, Vector2Int cellId)
        {
            ChangePartsArrayLength(1);
            var newSnakePart = Instantiate(partPrefab);
            _parts[^1] = newSnakePart;
            gameField.SetObjectCell(newSnakePart, cellId);
        }
        
        private void ChangePartsArrayLength(int count)
        {
            var tempParts = _parts;
            _parts = new GameFieldObject[tempParts.Length + count];
            for (var i = 0; i < _parts.Length; i++)
            {
                if (i >= tempParts.Length)
                {
                    break;
                }
                _parts[i] = tempParts[i];
            } 
        }
        
        private void GetMoveDirection()
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _moveDirection != Vector2Int.down)
            {
                SetMoveDirection(Vector2Int.up);
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && _moveDirection != Vector2Int.right)
            {
                SetMoveDirection(Vector2Int.left);
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && _moveDirection != Vector2Int.up)
            {
                SetMoveDirection(Vector2Int.down);
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && _moveDirection != Vector2Int.left)
            {
                SetMoveDirection(Vector2Int.right);
            }
        }
        
        private void SetMoveDirection(Vector2Int moveDirection)
        {
            _moveDirection = moveDirection;
            SetHeadRotation(moveDirection);
            Move();
        }
        
        private void MoveTimerTick()
        {
            _moveTimer += Time.deltaTime;
            if(_moveTimer >= moveDelay)
            {
                Move();
            }
        }
        
        private void Move()
        {
            _moveTimer = 0;
            var lastPartCellId = _parts[^1].GetCellId();
            var headNewCell = MoveCellId(_parts[0].GetCellId(), _moveDirection);
            gameField.SetCellIsEmpty(lastPartCellId.x, lastPartCellId.y, true);

            for (var i = _parts.Length - 1; i >= 0; i--)
            {
                var partCellId = i == 0 ? headNewCell : _parts[i - 1].GetCellId();
                gameField.SetObjectCell(_parts[i], partCellId);
            }
            CheckNextCellFail(headNewCell);
            CheckNextCellApple(headNewCell, lastPartCellId);
        }
        
        private Vector2Int MoveCellId(Vector2Int cellId, Vector2Int direction)
        {
            cellId += direction;
            
            if(cellId.x >= gameField.CellsInRow)
            {
                cellId.x = 0;
            }
            else if (cellId.x < 0)
            {
                cellId.x = gameField.CellsInRow - 1;
            }
            if (cellId.y >= gameField.CellsInRow)
            {
                cellId.y = 0;
            }
            else if (cellId.y < 0)
            {
                cellId.y = gameField.CellsInRow - 1;
            }
            return cellId;
        }
        
        private void SetHeadRotation(Vector2Int direction)
        {
            var headEuler = direction switch
            {
                { x: 0, y: 1 } => new Vector3(0f, 0f, 0f),
                { x: 1, y: 0 } => new Vector3(0f, 0f, -90f),
                { x: 0, y: -1 } => new Vector3(0f, 0f, 180f),
                { x: -1, y: 0 } => new Vector3(0f, 0f, 90f),
                _ => Vector3.zero
            };
            _parts[0].transform.eulerAngles = headEuler;
        }
    }
}