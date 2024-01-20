using UnityEngine;

namespace PlayForge_Team.Snake.Runtime
{
    public class GameField : MonoBehaviour
    {
        public int CellsInRow => cellsInRow;
        
        [SerializeField] private Transform firstCellPoint;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private int cellsInRow = 12;
        private GameFieldCell[,] _cells;
        
        public void FillCellsPositions()
        {
            _cells = new GameFieldCell[CellsInRow, CellsInRow];

            for (var i = 0; i < CellsInRow; i++)
            {
                for (var j = 0; j < CellsInRow; j++)
                {
                    var cellPosition = (Vector2)firstCellPoint.position + Vector2.right * i * cellSize.x +
                                       Vector2.up * j * cellSize.y;
                    var newCell = new GameFieldCell(cellPosition);
                    _cells[i,j] = newCell;
                }
            }
        }
        
        public Vector2 GetCellPosition(Vector2Int cellId)
        {
            return GetCellPosition(cellId.x, cellId.y);
        }

        private Vector2 GetCellPosition(int x, int y)
        {
            var cell = GetCell(x, y);
            return cell == null ? Vector2.zero : _cells[x, y].GetPosition();
        }
        
        public void SetObjectCell(GameFieldObject obj, Vector2Int newCellId)
        {
            var cellPosition = GetCellPosition(newCellId.x, newCellId.y);
            obj.SetCellPosition(newCellId, cellPosition);
            SetCellIsEmpty(newCellId.x, newCellId.y, false);
        }
        
        public bool GetCellIsEmpty(int x, int y)
        {
            var cell = GetCell(x, y);
            return cell != null &&
                   cell.GetIsEmpty();
        }

        public void SetCellIsEmpty(int x, int y, bool value)
        {
            var cell = GetCell(x, y);
            if (cell == null)
            {
                return;
            }
            _cells[x, y].SetIsEmpty(value);
        }

        private GameFieldCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= CellsInRow || y >= CellsInRow)
            {
                return null;
            }
            return _cells[x, y];
        }
    }
}
