using UnityEngine;

namespace PlayForge_Team.Snake.Runtime
{
    public class GameField : MonoBehaviour
    {
        [SerializeField] private Transform firstCellPoint;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private int cellsInRow = 12;
        private Vector2[,] _cellsPositions;
        
        public void FillCellsPositions()
        {
            _cellsPositions = new Vector2[cellsInRow, cellsInRow];
            for (var i = 0; i < cellsInRow; i++)
            {
                for (var j = 0; j < cellsInRow; j++)
                {
                    _cellsPositions[i, j] = (Vector2)firstCellPoint.position + Vector2.right * i * cellSize.x +
                                            Vector2.up * j * cellSize.y;
                }
            }
        }
        
        public Vector2 GetCellPosition(int x, int y)
        {
            if (x < 0 || y < 0 || x >= cellsInRow || y >= cellsInRow)
            {
                return Vector2.zero;
            }
            return _cellsPositions[x, y];
        }
    }
}
