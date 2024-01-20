using System;
using UnityEngine;

namespace PlayForge_Team.Snake.Runtime.Snakes
{
    public sealed class Snake : MonoBehaviour
    {
        [SerializeField] private GameField gameField;
        [SerializeField] private SnakePart headPrefab;
        [SerializeField] private SnakePart bodyPrefab;
        [SerializeField] private Vector2Int startCellId = new(5, 5);
        [SerializeField] private float moveDelay = 1.3f;
        private SnakePart[] _parts;
        private Vector2Int _moveDirection = Vector2Int.up;
        private float _moveTimer;
        
        private void Update()
        {
            GetMoveDirection();
            MoveTimerTick();
        }
        
        public void CreateSnake()
        {
            _parts = Array.Empty<SnakePart>();
            AddPart(headPrefab, startCellId);
            AddPart(bodyPrefab, startCellId + Vector2Int.down);
        }
        
        private void AddPart(SnakePart partPrefab, Vector2Int cellId)
        {
            IncreasePartsArrayLength();
            var newSnakePart = Instantiate(partPrefab);
            _parts[^1] = newSnakePart;
            SetPartCell(newSnakePart, cellId);
        }
        
        private void IncreasePartsArrayLength()
        {
            var tempParts = _parts;
            _parts = new SnakePart[tempParts.Length + 1];
            for (var i = 0; i < tempParts.Length; i++)
            {
                _parts[i] = tempParts[i];
            } 
        }
        
        private void SetPartCell(SnakePart part, Vector2Int cellId)
        {
            var cellPosition = gameField.GetCellPosition(cellId.x, cellId.y);
            part.SetCellPosition(cellId, cellPosition);
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
            for (var i = _parts.Length - 1; i >= 0; i--)
            {
                var partCellId = _parts[i].GetCellId();
                partCellId = i == 0 ? MoveCellId(partCellId, _moveDirection) : _parts[i - 1].GetCellId();
                SetPartCell(_parts[i], partCellId);
            }
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